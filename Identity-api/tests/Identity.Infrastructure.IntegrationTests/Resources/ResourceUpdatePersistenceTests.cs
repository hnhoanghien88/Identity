using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.IntegrationTests.Resources;

public sealed class ResourceUpdatePersistenceTests
{
    [Fact]
    public void Version_is_an_optimistic_concurrency_token()
    {
        var builder = new ModelBuilder();
        new ResourcesConfiguration().Configure(builder.Entity<Identity.Domain.Entities.Resources>());
        var version = builder.Model
            .FindEntityType(typeof(Identity.Domain.Entities.Resources))!
            .FindProperty(nameof(Identity.Domain.Entities.Resources.Version))!;

        Assert.True(version.IsConcurrencyToken);
    }
}