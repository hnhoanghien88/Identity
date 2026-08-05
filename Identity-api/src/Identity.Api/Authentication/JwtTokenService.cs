using System.IdentityModel.Tokens.Jwt;
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
}
public sealed record LoginTokens(string AccessToken, DateTime AccessTokenExpiresAtUtc, string RefreshToken, DateTime RefreshTokenExpiresAtUtc);

public sealed class JwtTokenService(IOptions<JwtOptions> options) : IJwtTokenService
{
    private readonly JwtOptions _options = options.Value;
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
        try
        {
            var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
            var principal = handler.ValidateToken(refreshToken, CreateValidationParameters(), out _);

            if (principal.FindFirst("token_type")?.Value != "refresh")
                throw new UnauthorizedAccessException("Only refresh tokens are accepted.");

            var subject = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (!ulong.TryParse(subject, out var userId))
                throw new UnauthorizedAccessException("The refresh token subject is invalid.");

            return userId;
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
