using System.IdentityModel.Tokens.Jwt;
using Identity.Api.Authentication;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Applications.Dtos;
using Identity.Application.Users.Dtos;
using Microsoft.Extensions.Options;

namespace Identity.Api.IntegrationTests.Authentication;

public sealed class JwtTokenServiceTests
{
    [Fact]
    public void CreateAccessToken_UsesApplicationScopeAndCompactClaims()
    {
        var service = new JwtTokenService(Options.Create(new JwtOptions
        {
            Issuer = "Identity.Api",
            Audience = "unused-default",
            ApplicationCode = "Identity",
            Key = "test-signing-key-that-is-at-least-32-bytes-long",
            AccessTokenMinutes = 10
        }));
        var user = new UsersDto(
            123,
            "user01",
            "user@example.com",
            "Example User",
            DateTime.UtcNow,
            true,
            8,
            1);
        var application = new ApplicationDto(
            7,
            "Business",
            "Business",
            "Business.Api",
            null,
            DateTime.UtcNow,
            true,
            1);

        var result = service.CreateAccessToken(
            user,
            new UserAuthorization(["Manager"], ["RestaurantProduct.View"]),
            application);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(result.Token);

        Assert.Equal("Business.Api", Assert.Single(jwt.Audiences));
        Assert.Equal("123", jwt.Subject);
        Assert.Equal("user01", jwt.Claims.Single(x => x.Type == "code").Value);
        Assert.Equal("7", jwt.Claims.Single(x => x.Type == "application_id").Value);
        Assert.Equal("Business", jwt.Claims.Single(x => x.Type == "application_code").Value);
        Assert.Equal("8", jwt.Claims.Single(x => x.Type == "permissionversion").Value);
        Assert.Equal("Manager", jwt.Claims.Single(x => x.Type == "role").Value);
        Assert.Equal("access", jwt.Claims.Single(x => x.Type == "token_type").Value);
        Assert.DoesNotContain(jwt.Claims, x => x.Type == "permission");
        Assert.DoesNotContain(jwt.Claims, x => x.Type is "email" or "display_name" or "jti");
    }
}
