using System.Reflection;
using Identity.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.IntegrationTests.Applications;

public sealed class ApplicationsControllerContractTests
{
    [Theory]
    [InlineData(nameof(ApplicationsController.Search), "Applications.Read")]
    [InlineData(nameof(ApplicationsController.GetById), "Applications.Read")]
    [InlineData(nameof(ApplicationsController.Create), "Applications.Create")]
    [InlineData(nameof(ApplicationsController.Update), "Applications.Update")]
    [InlineData(nameof(ApplicationsController.Delete), "Applications.Delete")]
    public void Endpoint_requires_expected_permission(string methodName, string policy)
    {
        var authorize = typeof(ApplicationsController)
            .GetMethod(methodName)!
            .GetCustomAttribute<AuthorizeAttribute>();

        Assert.NotNull(authorize);
        Assert.Equal(policy, authorize.Policy);
    }

    [Fact]
    public void Update_and_delete_require_version()
    {
        Assert.Contains(
            typeof(ApplicationsController.UpdateRequest).GetProperties(),
            property => property.Name == "Version"
                && property.PropertyType == typeof(ulong));
        var version = typeof(ApplicationsController)
            .GetMethod(nameof(ApplicationsController.Delete))!
            .GetParameters()
            .Single(parameter => parameter.Name == "version");
        Assert.NotNull(version.GetCustomAttribute<FromQueryAttribute>());
    }
}