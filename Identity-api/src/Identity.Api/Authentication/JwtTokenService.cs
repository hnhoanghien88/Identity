using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Applications.Dtos;
using Identity.Application.Users.Dtos;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Api.Authentication;

public interface IJwtTokenService
{
    AccessTokenResult CreateAccessToken(
        UsersDto user,
        UserAuthorization authorization,
        ApplicationDto application);
}

public sealed record AccessTokenResult(
    string Token,
    DateTime ExpiresAtUtc);

public sealed class JwtTokenService(IOptions<JwtOptions> options) : IJwtTokenService
{
    private readonly JwtOptions _options = options.Value;
    public AccessTokenResult CreateAccessToken(
        UsersDto user,
        UserAuthorization authorization,
        ApplicationDto application)
    {
        var now = DateTime.UtcNow;
        var expiry = now.AddMinutes(_options.AccessTokenMinutes);
        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new("code", user.Code),
            new("application_id", application.Id.ToString()),
            new("application_code", application.Code),
            new("permissionversion", user.PermissionVersion.ToString()),
            new("token_type", "access")
        ];

        claims.AddRange(authorization.Roles.Select(role =>
            new Claim("role", role)));

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
            _options.Issuer,
            application.Audience,
            claims,
            now,
            expiry,
            credentials));

        return new AccessTokenResult(token, expiry);
    }

}

