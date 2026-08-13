using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Users.Dtos;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Api.Authentication;

public interface IJwtTokenService
{
    AccessTokenResult CreateAccessToken(
        UsersDto user,
        UserAuthorization authorization);
    void RevokeAccessToken(ClaimsPrincipal principal);
    bool IsAccessTokenRevoked(ClaimsPrincipal principal);
}

public sealed record AccessTokenResult(
    string Token,
    DateTime ExpiresAtUtc);

public sealed class JwtTokenService(IOptions<JwtOptions> options) : IJwtTokenService
{
    private readonly JwtOptions _options = options.Value;
    private readonly ConcurrentDictionary<string, DateTime> _revokedTokens = new();

    public AccessTokenResult CreateAccessToken(
        UsersDto user,
        UserAuthorization authorization)
    {
        var now = DateTime.UtcNow;
        var expiry = now.AddMinutes(_options.AccessTokenMinutes);
        List<Claim> claims =
        [
            new("uid", user.Id.ToString()),
            new("permissionversion", user.PermissionVersion.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.Name),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("token_type", "access")
        ];

        claims.AddRange(authorization.Roles.Select(role =>
            new Claim("role", role)));

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            now,
            expiry,
            credentials));

        return new AccessTokenResult(token, expiry);
    }

    public void RevokeAccessToken(ClaimsPrincipal principal)
    {
        var jwtId = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        var expiration = principal.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;

        if (string.IsNullOrWhiteSpace(jwtId)
            || !long.TryParse(expiration, out var expirationSeconds))
        {
            throw new UnauthorizedAccessException("The token claims are invalid.");
        }

        _revokedTokens[jwtId] =
            DateTimeOffset.FromUnixTimeSeconds(expirationSeconds).UtcDateTime;

        foreach (var token in _revokedTokens.Where(x => x.Value <= DateTime.UtcNow))
            _revokedTokens.TryRemove(token.Key, out _);
    }

    public bool IsAccessTokenRevoked(ClaimsPrincipal principal)
    {
        var jwtId = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        return string.IsNullOrWhiteSpace(jwtId)
            || _revokedTokens.ContainsKey(jwtId);
    }
}

