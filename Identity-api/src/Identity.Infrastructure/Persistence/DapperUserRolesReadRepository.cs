using Dapper;
using Identity.Application.Abstractions.Persistence;

namespace Identity.Infrastructure.Persistence;

public sealed class DapperUserRolesReadRepository(MySqlConnectionFactory connectionFactory)
    : IUserRolesReadRepository
{
    public async Task<IReadOnlyList<string>> GetRoleCodesAsync(
        ulong userId,
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT DISTINCT r.Code
            FROM user_roles ur
            INNER JOIN roles r ON r.Id = ur.RoleId
            WHERE ur.UserId = @UserId
              AND ur.IsActive = TRUE
              AND r.IsActive = TRUE
              AND r.IsDeleted = FALSE
            ORDER BY r.Code
            """;

        await using var connection = connectionFactory.CreateConnection();
        var roles = await connection.QueryAsync<string>(new CommandDefinition(
            sql,
            new { UserId = userId },
            cancellationToken: cancellationToken));
        return roles.AsList();
    }
}
