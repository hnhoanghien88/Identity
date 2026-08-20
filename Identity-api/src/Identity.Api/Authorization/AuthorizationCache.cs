using Identity.Application.Abstractions.Persistence;
using Identity.Api.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Identity.Api.Authorization;

public interface IAuthorizationCache
{
    Task<UserAuthorization> GetAsync(
        ulong userId,
        int permissionVersion,
        CancellationToken cancellationToken);

    Task<UserAuthorization> GetAsync(
        ulong userId,
        int permissionVersion,
        string applicationCode,
        CancellationToken cancellationToken);
}

public sealed class AuthorizationCache(
    IMemoryCache cache,
    IUserRolesReadRepository repository,
    IOptions<JwtOptions> jwtOptions) : IAuthorizationCache
{
    public Task<UserAuthorization> GetAsync(
        ulong userId,
        int permissionVersion,
        CancellationToken cancellationToken)
    {
        return GetAsync(
            userId,
            permissionVersion,
            jwtOptions.Value.ApplicationCode,
            cancellationToken);
    }

    public Task<UserAuthorization> GetAsync(
        ulong userId,
        int permissionVersion,
        string applicationCode,
        CancellationToken cancellationToken)
    {
        var normalizedApplicationCode = applicationCode.Trim();
        if (normalizedApplicationCode.Length == 0)
            throw new ArgumentException(
                "Application code is required.",
                nameof(applicationCode));

        var key = $"authorization:{normalizedApplicationCode}:{userId}:{permissionVersion}";
        return cache.GetOrCreateAsync(
            key,
            entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
                entry.SlidingExpiration = TimeSpan.FromMinutes(5);
                return repository.GetAuthorizationAsync(
                    userId,
                    normalizedApplicationCode,
                    cancellationToken);
            })!;
    }
}
