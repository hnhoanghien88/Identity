namespace Identity.Application.Abstractions.Persistence;

public interface IUserRolesReadRepository
{
    Task<UserAuthorization> GetAuthorizationAsync(
        ulong userId,
        string applicationCode,
        CancellationToken cancellationToken);
}

public sealed record UserAuthorization(
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions);