using System.Text;
using Dapper;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Resources.Dtos;
using Identity.Application.Resources.GetResources;
using Identity.Application.Users.GetUsers;

namespace Identity.Infrastructure.Persistence;

public sealed class DapperResourcesReadRepository(MySqlConnectionFactory connectionFactory)
    : IResourcesReadRepository
{
    private const string SelectResources = """
        SELECT r.Id, r.ApplicationId, a.Code AS ApplicationCode,
               a.Name AS ApplicationName, r.Code, r.Name, r.ResourceType,
               r.Description, r.CreatedDate, r.IsActive, r.Version
        FROM resources r
        INNER JOIN applications a ON a.Id = r.ApplicationId
        """;

    public async Task<ResourceDto?> GetByIdAsync(
        ulong id,
        CancellationToken cancellationToken)
    {
        await using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<ResourceDto>(
            new CommandDefinition(
                $"{SelectResources} WHERE r.Id = @Id AND r.IsDeleted = 0",
                new { Id = id },
                cancellationToken: cancellationToken));
    }

    public async Task<PagedResourcesDto> GetAsync(
        ResourcesFilter filter,
        IReadOnlyList<ResourcesSort> sorts,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var where = new StringBuilder(" WHERE r.IsDeleted = 0");
        var parameters = new DynamicParameters();
        if (filter.ApplicationId.HasValue)
        {
            where.Append(" AND r.ApplicationId = @ApplicationId");
            parameters.Add("ApplicationId", filter.ApplicationId.Value);
        }
        AddStringFilter(
            where,
            parameters,
            "CONCAT(a.Code, ' ', a.Name)",
            "Application",
            filter.Application);
        AddStringFilter(where, parameters, "r.Code", "Code", filter.Code);
        AddStringFilter(where, parameters, "r.Name", "Name", filter.Name);
        AddStringFilter(
            where,
            parameters,
            "r.ResourceType",
            "ResourceType",
            filter.ResourceType);
        if (filter.IsActive.HasValue)
        {
            where.Append(" AND r.IsActive = @IsActive");
            parameters.Add("IsActive", filter.IsActive.Value);
        }

        var sql = new StringBuilder(SelectResources).Append(where);
        sql.Append(" ORDER BY ").Append(BuildOrderBy(sorts));
        sql.Append(" LIMIT @PageSize OFFSET @Offset");
        parameters.Add("PageSize", pageSize);
        parameters.Add("Offset", (page - 1) * pageSize);

        await using var connection = connectionFactory.CreateConnection();
        var resources = await connection.QueryAsync<ResourceDto>(
            new CommandDefinition(
                sql.ToString(),
                parameters,
                cancellationToken: cancellationToken));
        var totalCount = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                $"SELECT COUNT(*) FROM resources r INNER JOIN applications a ON a.Id = r.ApplicationId{where}",
                parameters,
                cancellationToken: cancellationToken));
        return new PagedResourcesDto(
            resources.AsList(),
            totalCount,
            page,
            pageSize);
    }

    private static void AddStringFilter(
        StringBuilder sql,
        DynamicParameters parameters,
        string expression,
        string parameterPrefix,
        StringFilter? filter)
    {
        if (filter?.Values is { Count: > 0 })
        {
            sql.Append($" AND {expression} IN @{parameterPrefix}Values");
            parameters.Add($"{parameterPrefix}Values", filter.Values);
        }
        AddLike(
            sql,
            parameters,
            expression,
            $"{parameterPrefix}Contains",
            filter?.Contains,
            value => $"%{value}%");
        AddLike(
            sql,
            parameters,
            expression,
            $"{parameterPrefix}StartsWith",
            filter?.StartsWith,
            value => $"{value}%");
        AddLike(
            sql,
            parameters,
            expression,
            $"{parameterPrefix}EndsWith",
            filter?.EndsWith,
            value => $"%{value}");
    }

    private static void AddLike(
        StringBuilder sql,
        DynamicParameters parameters,
        string expression,
        string parameterName,
        string? value,
        Func<string, string> pattern)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;
        sql.Append($" AND {expression} LIKE @{parameterName}");
        parameters.Add(parameterName, pattern(value.Trim()));
    }

    private static string BuildOrderBy(IReadOnlyList<ResourcesSort> sorts) =>
        string.Join(
            ", ",
            sorts.Select(sort =>
            {
                var column = sort.Column switch
                {
                    ResourcesSortColumn.Id => "r.Id",
                    ResourcesSortColumn.Application => "a.Code",
                    ResourcesSortColumn.Code => "r.Code",
                    ResourcesSortColumn.Name => "r.Name",
                    ResourcesSortColumn.ResourceType => "r.ResourceType",
                    ResourcesSortColumn.CreatedDate => "r.CreatedDate",
                    ResourcesSortColumn.IsActive => "r.IsActive",
                    _ => throw new ArgumentOutOfRangeException(nameof(sort.Column)),
                };
                var direction = sort.Direction == SortDirection.Descending
                    ? "DESC"
                    : "ASC";
                return $"{column} {direction}";
            }));
}
