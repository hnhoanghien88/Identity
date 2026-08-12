using Identity.Domain.Entities;

namespace Identity.Infrastructure.IntegrationTests.Resources;

public sealed class ResourceDeletePersistenceTests
{
    [Fact]
    public void Resource_supports_active_and_soft_delete_state()
    {
        var resource = new Identity.Domain.Entities.Resources
        {
            IsActive = false,
            IsDeleted = true,
        };

        Assert.False(resource.IsActive);
        Assert.True(resource.IsDeleted);
        Assert.IsAssignableFrom<ActiveEntity>(resource);
    }
}