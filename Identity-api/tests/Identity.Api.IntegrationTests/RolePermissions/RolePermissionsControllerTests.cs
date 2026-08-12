using Identity.Api.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace Identity.Api.IntegrationTests.RolePermissions;

public sealed class RolePermissionsControllerTests
{
    [Fact]
    public void Controller_requires_authentication_without_permission_policy()
    {
        var authorize = Assert.Single(
            typeof(RolePermissionsController).GetCustomAttributes(
                typeof(AuthorizeAttribute),
                true));
        Assert.Null(((AuthorizeAttribute)authorize).Policy);
        Assert.Null(((AuthorizeAttribute)authorize).Roles);
    }

    [Theory]
    [InlineData(nameof(RolePermissionsController.Get))]
    [InlineData(nameof(RolePermissionsController.Grant))]
    [InlineData(nameof(RolePermissionsController.Revoke))]
    public void Contract_exposes_each_operation(string methodName) =>
        Assert.NotNull(typeof(RolePermissionsController).GetMethod(methodName));
}
