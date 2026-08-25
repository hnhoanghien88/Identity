using Identity.Api.Authentication;

namespace Identity.Api.IntegrationTests.Authentication;

public sealed class RefreshTokenCookieTests
{
    [Fact]
    public void NameFor_IsStableAndIsolatedByApplication()
    {
        var business = RefreshTokenCookie.NameFor("Business");

        Assert.Equal(business, RefreshTokenCookie.NameFor(" business "));
        Assert.NotEqual(business, RefreshTokenCookie.NameFor("Identity"));
        Assert.StartsWith("identity_refresh_", business);
        Assert.Matches("^[a-z0-9_]+$", business);
    }

    [Fact]
    public void NameFor_RejectsMissingApplicationCode()
    {
        Assert.Throws<ArgumentException>(() => RefreshTokenCookie.NameFor(" "));
    }
}
