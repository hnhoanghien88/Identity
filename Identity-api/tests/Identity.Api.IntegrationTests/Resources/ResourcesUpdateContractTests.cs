using Identity.Api.Controllers;

namespace Identity.Api.IntegrationTests.Resources;

public sealed class ResourcesUpdateContractTests
{
    [Fact]
    public void Update_request_exposes_state_and_concurrency_fields()
    {
        var properties = typeof(ResourcesController.UpdateRequest)
            .GetProperties()
            .Select(value => value.Name)
            .ToArray();

        Assert.Contains("ApplicationId", properties);
        Assert.Contains("IsActive", properties);
        Assert.Contains("Version", properties);
    }
}