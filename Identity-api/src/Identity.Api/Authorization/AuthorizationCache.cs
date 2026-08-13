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
        var applicationCode = jwtOptions.Value.ApplicationCode;
        var key = $"authorization:{applicationCode}:{userId}:{permissionVersion}";
        return cache.GetOrCreateAsync(
            key,
            entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
                entry.SlidingExpiration = TimeSpan.FromMinutes(5);
                return repository.GetAuthorizationAsync(
                    userId,
                    applicationCode,
                    cancellationToken);
            })!;
    }
}
