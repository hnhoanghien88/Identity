using System.Text;
using Dapper;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Roles.Dtos;
using Identity.Application.Roles.GetRoles;
using Identity.Application.Users.GetUsers;

namespace Identity.Infrastructure.Persistence;

public sealed class DapperRolesReadRepository(MySqlConnectionFactory connectionFactory) : IRolesReadRepository
{
    private const string SelectRoles = """
        SELECT r.Id, r.ApplicationId, a.Code AS ApplicationCode, a.Name AS ApplicationName,
               r.Code, r.Name, r.IsSystemRole, r.IsActive, r.CreatedDate, r.Version
        FROM roles r
        INNER JOIN applications a ON a.Id = r.ApplicationId
        """;

    public async Task<RoleDto?> GetByIdAsync(ulong id, CancellationToken cancellationToken)
    {
        await using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<RoleDto>(
            new CommandDefinition($"{SelectRoles} WHERE r.Id = @Id", new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<PagedRolesDto> GetAsync(
        RolesFilter filter,
        IReadOnlyList<RolesSort> sorts,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var where = new StringBuilder(" WHERE r.IsDeleted = 0");
        var parameters = new DynamicParameters();
        if (filter.ApplicationIds is { Count: > 0 })
        {
            where.Append(" AND r.ApplicationId IN @ApplicationIds");
            parameters.Add("ApplicationIds", filter.ApplicationIds);
        }
        AddStringFilter(where, parameters, "r.Code", "Code", filter.Code);
        AddStringFilter(where, parameters, "r.Name", "Name", filter.Name);
        if (filter.IsSystemRole.HasValue)
        {
            where.Append(" AND r.IsSystemRole = @IsSystemRole");
            parameters.Add("IsSystemRole", filter.IsSystemRole.Value);
        }
        if (filter.IsActive.HasValue)
        {
            where.Append(" AND r.IsActive = @IsActive");
            parameters.Add("IsActive", filter.IsActive.Value);
        }
        var sql = new StringBuilder(SelectRoles).Append(where);
        sql.Append(" ORDER BY ").Append(BuildOrderBy(sorts));
        sql.Append(" LIMIT @PageSize OFFSET @Offset");
        parameters.Add("PageSize", pageSize);
        parameters.Add("Offset", (page - 1) * pageSize);
        await using var connection = connectionFactory.CreateConnection();
        var roles = await connection.QueryAsync<RoleDto>(
            new CommandDefinition(sql.ToString(), parameters, cancellationToken: cancellationToken));
        var totalCount = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                $"SELECT COUNT(*) FROM roles r{where}",
                parameters,
                cancellationToken: cancellationToken));
        return new PagedRolesDto(roles.AsList(), totalCount, page, pageSize);
    }

    private static void AddStringFilter(
        StringBuilder sql,
        DynamicParameters parameters,
        string column,
        string key,
        StringFilter? filter)
    {
        if (filter?.Values is { Count: > 0 })
        {
            sql.Append($" AND {column} IN @{key}Values");
            parameters.Add($"{key}Values", filter.Values);
        }
        AddLike(sql, parameters, column, $"{key}Contains", filter?.Contains, value => $"%{value}%");
        AddLike(sql, parameters, column, $"{key}StartsWith", filter?.StartsWith, value => $"{value}%");
        AddLike(sql, parameters, column, $"{key}EndsWith", filter?.EndsWith, value => $"%{value}");
    }

    private static void AddLike(
        StringBuilder sql,
        DynamicParameters parameters,
        string column,
        string parameterName,
        string? value,
        Func<string, string> pattern)
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        sql.Append($" AND {column} LIKE @{parameterName}");
        parameters.Add(parameterName, pattern(value.Trim()));
    }

    private static string BuildOrderBy(IReadOnlyList<RolesSort> sorts) =>
        string.Join(", ", sorts.Select(sort =>
        {
            var column = sort.Column switch
            {
                RolesSortColumn.Id => "r.Id",
                RolesSortColumn.Application => "a.Name",
                RolesSortColumn.Code => "r.Code",
                RolesSortColumn.Name => "r.Name",
                RolesSortColumn.IsSystemRole => "r.IsSystemRole",
                RolesSortColumn.IsActive => "r.IsActive",
                RolesSortColumn.CreatedDate => "r.CreatedDate",
                _ => throw new ArgumentOutOfRangeException(nameof(sort.Column)),
            };
            return $"{column} {(sort.Direction == SortDirection.Descending ? "DESC" : "ASC")}";
        }));
}
