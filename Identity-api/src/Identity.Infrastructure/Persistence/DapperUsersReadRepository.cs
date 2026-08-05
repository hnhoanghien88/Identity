using System.Text;
using Dapper;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Users.Dtos;
using Identity.Application.Users.GetUsers;

namespace Identity.Infrastructure.Persistence;

public sealed class DapperUsersReadRepository(MySqlConnectionFactory connectionFactory)
    : IUsersReadRepository
{
    private const string SelectUsers = """
        SELECT Id, Email AS Code, DisplayName AS Name, CreatedDate, IsActive
        FROM users
        """;

    public async Task<UsersDto?> GetByIdAsync(ulong id, CancellationToken cancellationToken)
    {
        await using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<UsersDto>(
            new CommandDefinition(
                $"{SelectUsers} WHERE Id = @Id",
                new { Id = id },
                cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<UsersDto>> GetAsync(
        UsersFilter filter,
        IReadOnlyList<UsersSort> sorts,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var sql = new StringBuilder(SelectUsers).AppendLine(" WHERE 1 = 1");
        var parameters = new DynamicParameters();

        AddFilters(sql, parameters, filter);
        sql.Append(" ORDER BY ").Append(BuildOrderBy(sorts));
        sql.Append(" LIMIT @PageSize OFFSET @Offset");
        parameters.Add("PageSize", pageSize);
        parameters.Add("Offset", (page - 1) * pageSize);

        await using var connection = connectionFactory.CreateConnection();
        var users = await connection.QueryAsync<UsersDto>(
            new CommandDefinition(sql.ToString(), parameters, cancellationToken: cancellationToken));
        return users.AsList();
    }

    private static void AddFilters(StringBuilder sql, DynamicParameters parameters, UsersFilter filter)
    {
        if (filter.Ids is { Count: > 0 })
        {
            sql.AppendLine(" AND Id IN @Ids");
            parameters.Add("Ids", filter.Ids);
        }

        AddStringFilter(sql, parameters, "Email", filter.Code);
        AddStringFilter(sql, parameters, "DisplayName", filter.Name);

        AddFilter(sql, parameters, "CreatedDate >= @CreatedDateFrom", "CreatedDateFrom", filter.CreatedDateFrom);
        AddFilter(sql, parameters, "CreatedDate <= @CreatedDateTo", "CreatedDateTo", filter.CreatedDateTo);
        AddFilter(sql, parameters, "IsActive = @IsActive", "IsActive", filter.IsActive);
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
        parameters.Add(parameterName, pattern(value));
    }

    private static void AddFilter<T>(
        StringBuilder sql,
        DynamicParameters parameters,
        string condition,
        string parameterName,
        T? value) where T : struct
    {
        if (!value.HasValue) return;
        sql.Append(" AND ").Append(condition);
        parameters.Add(parameterName, value.Value);
    }

    private static string BuildOrderBy(IReadOnlyList<UsersSort> sorts)
    {
        if (sorts.Count == 0) return "Id ASC";

        return string.Join(", ", sorts.Select(sort =>
        {
            var column = sort.Column switch
            {
                UsersSortColumn.Id => "Id",
                UsersSortColumn.Code => "Email",
                UsersSortColumn.Name => "DisplayName",
                UsersSortColumn.CreatedDate => "CreatedDate",
                UsersSortColumn.IsActive => "IsActive",
                _ => throw new ArgumentOutOfRangeException(nameof(sort.Column))
            };
            var direction = sort.Direction == SortDirection.Descending ? "DESC" : "ASC";
            return $"{column} {direction}";
        }));
    }
}
