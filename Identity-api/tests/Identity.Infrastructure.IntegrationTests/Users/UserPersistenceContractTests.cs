using Identity.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using UserEntity = Identity.Domain.Entities.Users;

namespace Identity.Infrastructure.IntegrationTests.Users;

public sealed class UserPersistenceContractTests
{
    [Fact]
    public void User_version_is_required_and_concurrent()
    {
        var entity = BuildEntity();
        var property = entity.FindProperty(nameof(UserEntity.Version))!;

        Assert.False(property.IsNullable);
        Assert.True(property.IsConcurrencyToken);
    }

    [Theory]
    [InlineData(nameof(UserEntity.Code), "utf8mb4_unicode_ci")]
    [InlineData(nameof(UserEntity.Email), "utf8mb4_unicode_ci")]
    public void Direct_identity_is_required_case_insensitive_and_unique(
        string propertyName,
        string collation)
    {
        var entity = BuildEntity();
        var property = entity.FindProperty(propertyName)!;
        var index = entity.GetIndexes().Single(
            value => value.Properties.Contains(property));

        Assert.False(property.IsNullable);
        Assert.Equal(collation, property.GetCollation());
        Assert.True(index.IsUnique);
    }

    [Fact]
    public void Normalized_identity_properties_are_absent()
    {
        var entity = BuildEntity();

        Assert.Null(entity.FindProperty("NormalizedCode"));
        Assert.Null(entity.FindProperty("NormalizedEmail"));
    }

    private static Microsoft.EntityFrameworkCore.Metadata.IMutableEntityType BuildEntity()
    {
        var builder = new ModelBuilder();
        new UsersConfiguration().Configure(builder.Entity<UserEntity>());
        return builder.Model.FindEntityType(typeof(UserEntity))!;
    }
}
