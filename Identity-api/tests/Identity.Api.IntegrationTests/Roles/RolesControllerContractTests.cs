using Identity.Api.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace Identity.Api.IntegrationTests.Roles;

public sealed class RolesControllerContractTests
{
    [Fact]
    public void Controller_requires_authentication_without_role_policy()
    {
        var authorize = Assert.Single(typeof(RolesController).GetCustomAttributes(typeof(AuthorizeAttribute), true));
        Assert.Null(((AuthorizeAttribute)authorize).Policy);
        Assert.Null(((AuthorizeAttribute)authorize).Roles);
    }
}
