using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.Application.Users.Dtos;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Api.Authentication;

public interface IJwtTokenService { LoginTokens CreateTokens(UsersDto user); }
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
