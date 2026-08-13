using System.Reflection;
using Identity.Api.Controllers;
using Identity.Application.Common.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.IntegrationTests.Resources;

public sealed class ResourcesReadContractTests
{
    [Theory]
    [InlineData(nameof(ResourcesController.Search), ResourcePermissions.Read)]
    [InlineData(nameof(ResourcesController.GetById), ResourcePermissions.Read)]
    [InlineData(nameof(ResourcesController.Create), ResourcePermissions.Create)]
    [InlineData(nameof(ResourcesController.Update), ResourcePermissions.Update)]
    [InlineData(nameof(ResourcesController.Delete), ResourcePermissions.Delete)]
    public void Endpoint_requires_expected_permission(string methodName, string policy)
    {
        var attribute = typeof(ResourcesController)
            .GetMethod(methodName)!
            .GetCustomAttribute<AuthorizeAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal(policy, attribute.Policy);
    }

    [Fact]
    public void Controller_uses_expected_route_and_requires_version()
    {
        var type = typeof(ResourcesController);
        Assert.Equal("api/resources", type.GetCustomAttribute<RouteAttribute>()!.Template);
        Assert.Contains(
            typeof(ResourcesController.UpdateRequest).GetProperties(),
            property => property.Name == "Version"
                && property.PropertyType == typeof(ulong));
        var version = type
            .GetMethod(nameof(ResourcesController.Delete))!
            .GetParameters()
            .Single(parameter => parameter.Name == "version");
        Assert.NotNull(version.GetCustomAttribute<FromQueryAttribute>());
    }
}