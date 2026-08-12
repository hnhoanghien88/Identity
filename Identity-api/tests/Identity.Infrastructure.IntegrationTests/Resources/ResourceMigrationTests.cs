using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.IntegrationTests.Resources;

public sealed class ResourceMigrationTests
{
    [Fact]
    public void Resource_schema_enforces_lengths_unique_scope_and_concurrency()
    {
        var builder = new ModelBuilder();
        new ResourcesConfiguration().Configure(builder.Entity<Identity.Domain.Entities.Resources>());
        var entity = builder.Model.FindEntityType(typeof(Identity.Domain.Entities.Resources))!;

        var code = entity.FindProperty(nameof(Identity.Domain.Entities.Resources.Code))!;
        var version = entity.FindProperty(nameof(Identity.Domain.Entities.Resources.Version))!;

        Assert.Equal(120, code.GetMaxLength());
        Assert.True(entity.GetIndexes().Single().IsUnique);
        Assert.Equal(
            ["ApplicationId", "Code"],
            entity.GetIndexes().Single().Properties.Select(value => value.Name));
        Assert.True(version.IsConcurrencyToken);
        Assert.Equal(1UL, version.GetDefaultValue());
        Assert.True(typeof(ActiveEntity).IsAssignableFrom(typeof(Identity.Domain.Entities.Resources)));
    }
}