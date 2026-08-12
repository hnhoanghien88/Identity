using System.Reflection;
using Identity.Api.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.IntegrationTests.Resources;

public sealed class ResourcesDeleteContractTests
{
    [Fact]
    public void Delete_requires_version_from_query()
    {
        var version = typeof(ResourcesController)
            .GetMethod(nameof(ResourcesController.Delete))!
            .GetParameters()
            .Single(value => value.Name == "version");

        Assert.Equal(typeof(ulong), version.ParameterType);
        Assert.NotNull(version.GetCustomAttribute<FromQueryAttribute>());
    }
}