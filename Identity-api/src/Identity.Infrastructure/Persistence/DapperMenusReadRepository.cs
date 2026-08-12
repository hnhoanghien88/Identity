using Dapper;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Menus.Dtos;

namespace Identity.Infrastructure.Persistence;

public sealed class DapperMenusReadRepository(MySqlConnectionFactory connectionFactory) : IMenusReadRepository
{
    private const string SelectMenus = """
        SELECT m.Id, m.ApplicationId, a.Name AS ApplicationName, m.ParentId,
               m.ResourceId, r.Name AS ResourceName, m.Code, m.Name, m.Route,
               m.Icon, m.SortOrder, m.IsVisible, m.IsActive, m.Version
        FROM menus m
        INNER JOIN applications a ON a.Id = m.ApplicationId
        LEFT JOIN resources r ON r.Id = m.ResourceId AND r.IsDeleted = 0
        """;

    public async Task<IReadOnlyList<MenuRowDto>> GetByApplicationAsync(ulong applicationId, CancellationToken cancellationToken)
    {
        await using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<MenuRowDto>(new CommandDefinition(
            $"{SelectMenus} WHERE m.ApplicationId = @ApplicationId AND m.IsDeleted = 0 ORDER BY m.SortOrder, m.Name, m.Id",
            new { ApplicationId = applicationId }, cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public async Task<MenuRowDto?> GetByIdAsync(ulong id, CancellationToken cancellationToken)
    {
        await using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<MenuRowDto>(new CommandDefinition(
            $"{SelectMenus} WHERE m.Id = @Id AND m.IsDeleted = 0", new { Id = id }, cancellationToken: cancellationToken));
    }
}
