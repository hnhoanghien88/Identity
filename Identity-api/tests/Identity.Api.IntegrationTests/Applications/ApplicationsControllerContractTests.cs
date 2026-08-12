using System.Reflection;
using Identity.Api.Controllers;
using Identity.Application.Common.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.IntegrationTests.Applications;

public sealed class ApplicationsControllerContractTests
{
    [Theory]
    [InlineData(nameof(ApplicationsController.Search))]
    [InlineData(nameof(ApplicationsController.GetById))]
    [InlineData(nameof(ApplicationsController.Create))]
    [InlineData(nameof(ApplicationsController.Update))]
    [InlineData(nameof(ApplicationsController.Delete))]
    public void Endpoint_inherits_authenticated_controller_access(string methodName)
    {
        var method = typeof(ApplicationsController).GetMethod(methodName)!;
        Assert.Empty(method.GetCustomAttributes<AllowAnonymousAttribute>());
        Assert.Contains(
            typeof(ApplicationsController).GetCustomAttributes<AuthorizeAttribute>(),
            _ => true);
    }

    [Fact]
    public void Update_and_delete_require_version()
    {
        Assert.Contains(
            typeof(ApplicationsController.UpdateRequest).GetProperties(),
            property => property.Name == "Version" && property.PropertyType == typeof(ulong));

        var version = typeof(ApplicationsController)
            .GetMethod(nameof(ApplicationsController.Delete))!
            .GetParameters()
            .Single(parameter => parameter.Name == "version");
        Assert.NotNull(version.GetCustomAttribute<FromQueryAttribute>());
    }

    [Fact]
    public void Controller_is_authenticated_and_uses_expected_route()
    {
        var type = typeof(ApplicationsController);
        Assert.Contains(type.GetCustomAttributes<AuthorizeAttribute>(), _ => true);
        Assert.Equal("api/applications", type.GetCustomAttribute<RouteAttribute>()!.Template);
    }
}
