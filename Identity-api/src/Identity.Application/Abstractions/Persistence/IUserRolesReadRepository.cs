namespace Identity.Application.Abstractions.Persistence;

public interface IUserRolesReadRepository
{
    Task<UserAuthorization> GetAuthorizationAsync(
        ulong userId,
        CancellationToken cancellationToken);
}

public sealed record UserAuthorization(
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions);