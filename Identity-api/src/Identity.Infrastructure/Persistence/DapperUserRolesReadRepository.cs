using Dapper;
using Identity.Application.Abstractions.Persistence;

namespace Identity.Infrastructure.Persistence;

public sealed class DapperUserRolesReadRepository(MySqlConnectionFactory connectionFactory)
    : IUserRolesReadRepository
{
    public async Task<UserAuthorization> GetAuthorizationAsync(
        ulong userId,
        CancellationToken cancellationToken)
    {
        const string rolesSql = """
            SELECT DISTINCT r.Code
            FROM user_roles ur
            INNER JOIN roles r ON r.Id = ur.RoleId
            WHERE ur.UserId = @UserId
              AND ur.IsActive = TRUE
              AND r.IsActive = TRUE
              AND r.IsDeleted = FALSE
            ORDER BY r.Code
            """;

        const string permissionsSql = """
            SELECT DISTINCT p.Code
            FROM user_roles ur
            INNER JOIN roles r ON r.Id = ur.RoleId
            INNER JOIN role_permissions rp ON rp.RoleId = r.Id
            INNER JOIN permissions p ON p.Id = rp.PermissionId
            WHERE ur.UserId = @UserId
              AND ur.IsActive = TRUE
              AND r.IsActive = TRUE
              AND r.IsDeleted = FALSE
              AND p.IsActive = TRUE
              AND p.IsDeleted = FALSE
            ORDER BY p.Code
            """;

        await using var connection = connectionFactory.CreateConnection();
        var command = new { UserId = userId };
        var roles = await connection.QueryAsync<string>(new CommandDefinition(
            rolesSql, command, cancellationToken: cancellationToken));
        var permissions = await connection.QueryAsync<string>(new CommandDefinition(
            permissionsSql, command, cancellationToken: cancellationToken));

        return new UserAuthorization(roles.AsList(), permissions.AsList());
    }
}