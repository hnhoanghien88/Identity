using Identity.Application.Abstractions.Persistence;
using Identity.Infrastructure.Persistence;

namespace Identity.Infrastructure.IntegrationTests.Resources;

public sealed class ResourceCreatePersistenceTests
{
    [Fact]
    public void Ef_repository_implements_write_contract()
    {
        Assert.True(
            typeof(IResourcesRepository).IsAssignableFrom(
                typeof(MySqlResourcesRepository)));
    }
}