using Identity.Api.Controllers;

namespace Identity.Api.IntegrationTests.Resources;

public sealed class ResourcesCreateContractTests
{
    [Fact]
    public void Create_request_exposes_required_resource_fields()
    {
        var properties = typeof(ResourcesController.CreateRequest)
            .GetProperties()
            .Select(value => value.Name)
            .ToArray();

        Assert.Equal(
            ["ApplicationId", "Code", "Name", "ResourceType", "Description"],
            properties);
    }
}