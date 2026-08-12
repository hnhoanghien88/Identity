using System.Text;
using Dapper;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Applications.Dtos;
using Identity.Application.Applications.GetApplications;
using Identity.Application.Users.GetUsers;

namespace Identity.Infrastructure.Persistence;

public sealed class DapperApplicationsReadRepository(MySqlConnectionFactory connectionFactory)
    : IApplicationsReadRepository
{
    private const string SelectApplications = """
        SELECT Id, Code, Name, Audience, Description, CreatedDate, IsActive, Version
        FROM applications
        """;

    public async Task<ApplicationDto?> GetByIdAsync(ulong id, CancellationToken cancellationToken)
    {
        await using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<ApplicationDto>(
            new CommandDefinition(
                $"{SelectApplications} WHERE Id = @Id AND IsDeleted = 0",
                new { Id = id },
                cancellationToken: cancellationToken));
    }

    public async Task<PagedApplicationsDto> GetAsync(
        ApplicationsFilter filter,
        IReadOnlyList<ApplicationsSort> sorts,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var where = new StringBuilder(" WHERE IsDeleted = 0");
        var parameters = new DynamicParameters();
        AddStringFilter(where, parameters, "Code", filter.Code);
        AddStringFilter(where, parameters, "Name", filter.Name);
        AddStringFilter(where, parameters, "Audience", filter.Audience);
        if (filter.IsActive.HasValue)
        {
            where.Append(" AND IsActive = @IsActive");
            parameters.Add("IsActive", filter.IsActive.Value);
        }

        var sql = new StringBuilder(SelectApplications).Append(where);
        sql.Append(" ORDER BY ").Append(BuildOrderBy(sorts));
        sql.Append(" LIMIT @PageSize OFFSET @Offset");
        parameters.Add("PageSize", pageSize);
        parameters.Add("Offset", (page - 1) * pageSize);

        await using var connection = connectionFactory.CreateConnection();
        var applications = await connection.QueryAsync<ApplicationDto>(
            new CommandDefinition(sql.ToString(), parameters, cancellationToken: cancellationToken));
        var totalCount = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                $"SELECT COUNT(*) FROM applications{where}",
                parameters,
                cancellationToken: cancellationToken));
        return new PagedApplicationsDto(applications.AsList(), totalCount, page, pageSize);
    }

    private static void AddStringFilter(
        StringBuilder sql,
        DynamicParameters parameters,
        string column,
        StringFilter? filter)
    {
        if (filter?.Values is { Count: > 0 })
        {
            sql.Append($" AND {column} IN @{column}Values");
            parameters.Add($"{column}Values", filter.Values);
        }
        AddLike(sql, parameters, column, "Contains", filter?.Contains, value => $"%{value}%");
        AddLike(sql, parameters, column, "StartsWith", filter?.StartsWith, value => $"{value}%");
        AddLike(sql, parameters, column, "EndsWith", filter?.EndsWith, value => $"%{value}");
    }

    private static void AddLike(
        StringBuilder sql,
        DynamicParameters parameters,
        string column,
        string suffix,
        string? value,
        Func<string, string> pattern)
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        var parameterName = $"{column}{suffix}";
        sql.Append($" AND {column} LIKE @{parameterName}");
        parameters.Add(parameterName, pattern(value.Trim()));
    }

    private static string BuildOrderBy(IReadOnlyList<ApplicationsSort> sorts) =>
        string.Join(
            ", ",
            sorts.Select(sort =>
            {
                var column = sort.Column switch
                {
                    ApplicationsSortColumn.Id => "Id",
                    ApplicationsSortColumn.Code => "Code",
                    ApplicationsSortColumn.Name => "Name",
                    ApplicationsSortColumn.Audience => "Audience",
                    ApplicationsSortColumn.CreatedDate => "CreatedDate",
                    ApplicationsSortColumn.IsActive => "IsActive",
                    _ => throw new ArgumentOutOfRangeException(nameof(sort.Column)),
                };
                var direction = sort.Direction == SortDirection.Descending ? "DESC" : "ASC";
                return $"{column} {direction}";
            }));
}
