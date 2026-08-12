using Identity.Application.Abstractions.Persistence;
using Identity.Infrastructure.Persistence;

namespace Identity.Infrastructure.IntegrationTests.Resources;

public sealed class ResourceReadPersistenceTests
{
    [Fact]
    public void Dapper_repository_implements_read_contract()
    {
        Assert.True(
            typeof(IResourcesReadRepository).IsAssignableFrom(
                typeof(DapperResourcesReadRepository)));
    }
}