using System.IdentityModel.Tokens.Jwt;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text;
using Identity.Application.Users.Dtos;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Api.Authentication;

public interface IJwtTokenService
{
    LoginTokens CreateTokens(UsersDto user);
    ulong ValidateRefreshToken(string refreshToken);
    void RevokeRefreshToken(string refreshToken);
    void RevokeAccessToken(ClaimsPrincipal principal);
    bool IsAccessTokenRevoked(ClaimsPrincipal principal);
}
public sealed record LoginTokens(string AccessToken, DateTime AccessTokenExpiresAtUtc, string RefreshToken, DateTime RefreshTokenExpiresAtUtc);

public sealed class JwtTokenService(IOptions<JwtOptions> options) : IJwtTokenService
{
    private readonly JwtOptions _options = options.Value;
    private readonly ConcurrentDictionary<string, DateTime> _revokedTokens = new();
    public LoginTokens CreateTokens(UsersDto user)
    {
        var now = DateTime.UtcNow;
        var accessExpiry = now.AddMinutes(_options.AccessTokenMinutes);
        var refreshExpiry = now.AddDays(_options.RefreshTokenDays);
        return new(CreateToken(user, "access", now, accessExpiry), accessExpiry,
            CreateToken(user, "refresh", now, refreshExpiry), refreshExpiry);
    }

    public ulong ValidateRefreshToken(string refreshToken)
    {
        var principal = ValidateRefreshTokenPrincipal(refreshToken);
        var jwtId = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

        if (string.IsNullOrWhiteSpace(jwtId) || _revokedTokens.ContainsKey(jwtId))
            throw new UnauthorizedAccessException("The refresh token has been revoked.");

        var subject = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!ulong.TryParse(subject, out var userId))
            throw new UnauthorizedAccessException("The refresh token subject is invalid.");

        return userId;
    }

    public void RevokeRefreshToken(string refreshToken)
    {
        var principal = ValidateRefreshTokenPrincipal(refreshToken);
        RevokeToken(principal);
    }

    public void RevokeAccessToken(ClaimsPrincipal principal) => RevokeToken(principal);

    public bool IsAccessTokenRevoked(ClaimsPrincipal principal)
    {
        var jwtId = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        return string.IsNullOrWhiteSpace(jwtId) || _revokedTokens.ContainsKey(jwtId);
    }

    private void RevokeToken(ClaimsPrincipal principal)
    {
        var jwtId = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        var expiration = principal.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;

        if (string.IsNullOrWhiteSpace(jwtId) || !long.TryParse(expiration, out var expirationSeconds))
            throw new UnauthorizedAccessException("The token claims are invalid.");

        _revokedTokens[jwtId] = DateTimeOffset.FromUnixTimeSeconds(expirationSeconds).UtcDateTime;

        foreach (var token in _revokedTokens.Where(x => x.Value <= DateTime.UtcNow))
            _revokedTokens.TryRemove(token.Key, out _);
    }

    private ClaimsPrincipal ValidateRefreshTokenPrincipal(string refreshToken)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
            var principal = handler.ValidateToken(refreshToken, CreateValidationParameters(), out _);

            if (principal.FindFirst("token_type")?.Value != "refresh")
                throw new UnauthorizedAccessException("Only refresh tokens are accepted.");

            return principal;
        }
        catch (SecurityTokenException exception)
        {
            throw new UnauthorizedAccessException("The refresh token is invalid or expired.", exception);
        }
        catch (ArgumentException exception)
        {
            throw new UnauthorizedAccessException("The refresh token is invalid.", exception);
        }
    }

    private TokenValidationParameters CreateValidationParameters() => new()
    {
        ValidateIssuer = true,
        ValidIssuer = _options.Issuer,
        ValidateAudience = true,
        ValidAudience = _options.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    private string CreateToken(UsersDto user, string type, DateTime now, DateTime expiry)
    {
        Claim[] claims = [new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Code), new(ClaimTypes.Name, user.Name),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), new("token_type", type)];
        var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key)), SecurityAlgorithms.HmacSha256);
        return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(_options.Issuer, _options.Audience,
            claims, now, expiry, credentials));
    }
}
