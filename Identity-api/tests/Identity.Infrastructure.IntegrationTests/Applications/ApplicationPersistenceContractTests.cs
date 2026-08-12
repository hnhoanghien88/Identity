using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.IntegrationTests.Applications;

public sealed class ApplicationPersistenceContractTests
{
    [Fact]
    public void Application_schema_enforces_lengths_uniqueness_and_concurrency()
    {
        var builder = new ModelBuilder();
        new ApplicationsConfiguration().Configure(builder.Entity<Identity.Domain.Entities.Applications>());
        var entity = builder.Model.FindEntityType(typeof(Identity.Domain.Entities.Applications))!;

        var code = entity.FindProperty(nameof(Identity.Domain.Entities.Applications.Code))!;
        var version = entity.FindProperty(nameof(Identity.Domain.Entities.Applications.Version))!;

        Assert.False(code.IsNullable);
        Assert.Equal(50, code.GetMaxLength());
        Assert.Equal("utf8mb4_unicode_ci", code.GetCollation());
        Assert.True(entity.GetIndexes().Single(index => index.Properties.Contains(code)).IsUnique);
        Assert.False(version.IsNullable);
        Assert.True(version.IsConcurrencyToken);
        Assert.Equal(1UL, version.GetDefaultValue());
        Assert.Equal(150, entity.FindProperty(nameof(Identity.Domain.Entities.Applications.Name))!.GetMaxLength());
        Assert.Equal(150, entity.FindProperty(nameof(Identity.Domain.Entities.Applications.Audience))!.GetMaxLength());
        Assert.Equal(500, entity.FindProperty(nameof(Identity.Domain.Entities.Applications.Description))!.GetMaxLength());
    }

    [Fact]
    public void Application_remains_an_active_soft_delete_entity()
    {
        Assert.True(typeof(ActiveEntity).IsAssignableFrom(typeof(Identity.Domain.Entities.Applications)));
    }
}
