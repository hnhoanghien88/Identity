using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.IntegrationTests.ExternalIdentities;

public sealed class ExternalIdentityConfigurationTests
{
    [Fact]
    public void Schema_enforces_provider_subject_and_user_provider_uniqueness()
    {
        var builder = new ModelBuilder();
        new ExternalIdentityConfiguration().Configure(builder.Entity<ExternalIdentity>());
        var entity = builder.Model.FindEntityType(typeof(ExternalIdentity))!;
        var indexes = entity.GetIndexes().ToDictionary(x => x.GetDatabaseName()!);

        Assert.True(indexes["UQExternalIdentitiesProviderSubject"].IsUnique);
        Assert.Equal(
            [nameof(ExternalIdentity.Provider), nameof(ExternalIdentity.ProviderSubject)],
            indexes["UQExternalIdentitiesProviderSubject"].Properties.Select(x => x.Name));
        Assert.True(indexes["UQExternalIdentitiesUserProvider"].IsUnique);
        Assert.Equal(
            [nameof(ExternalIdentity.UserId), nameof(ExternalIdentity.Provider)],
            indexes["UQExternalIdentitiesUserProvider"].Properties.Select(x => x.Name));
        Assert.Equal(254, entity.FindProperty(nameof(ExternalIdentity.EmailAtLinkTime))!.GetMaxLength());
    }
}
