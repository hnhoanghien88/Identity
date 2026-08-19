using Dapper;
using Identity.Application.Abstractions.Persistence;

namespace Identity.Infrastructure.Persistence;

public sealed class DapperUserRolesReadRepository(MySqlConnectionFactory connectionFactory)
    : IUserRolesReadRepository
{
    public async Task<UserAuthorization> GetAuthorizationAsync(
        ulong userId,
        string applicationCode,
        CancellationToken cancellationToken)
    {
        const string rolesSql = """
            SELECT DISTINCT r.Code
            FROM user_roles ur
            INNER JOIN roles r ON r.Id = ur.RoleId
            INNER JOIN applications a ON a.Id = r.ApplicationId
            WHERE ur.UserId = @UserId
              AND ur.IsActive = TRUE
              AND ur.IsDeleted = FALSE
              AND r.IsActive = TRUE
              AND r.IsDeleted = FALSE
              AND a.Code = @ApplicationCode
              AND a.IsActive = TRUE
              AND a.IsDeleted = FALSE
            ORDER BY r.Code
            """;

        const string permissionsSql = """
            SELECT DISTINCT p.Code
            FROM user_roles ur
            INNER JOIN roles r ON r.Id = ur.RoleId
            INNER JOIN role_permissions rp ON rp.RoleId = r.Id
            INNER JOIN permissions p ON p.Id = rp.PermissionId
            INNER JOIN resources resource ON resource.Id = p.ResourceId
            INNER JOIN applications a ON a.Id = resource.ApplicationId
            WHERE ur.UserId = @UserId
              AND ur.IsActive = TRUE
              AND ur.IsDeleted = FALSE
              AND r.IsActive = TRUE
              AND r.IsDeleted = FALSE
              AND rp.IsActive = TRUE
              AND rp.IsDeleted = FALSE
              AND p.IsActive = TRUE
              AND p.IsDeleted = FALSE
              AND resource.IsActive = TRUE
              AND resource.IsDeleted = FALSE
              AND r.ApplicationId = resource.ApplicationId
              AND a.Code = @ApplicationCode
              AND a.IsActive = TRUE
              AND a.IsDeleted = FALSE
            ORDER BY p.Code
            """;

        await using var connection = connectionFactory.CreateConnection();
        var command = new { UserId = userId, ApplicationCode = applicationCode };
        var roles = await connection.QueryAsync<string>(new CommandDefinition(
            rolesSql,
            command,
            cancellationToken: cancellationToken));
        var permissions = await connection.QueryAsync<string>(new CommandDefinition(
            permissionsSql,
            command,
            cancellationToken: cancellationToken));
        return new UserAuthorization(roles.AsList(), permissions.AsList());
    }
}
