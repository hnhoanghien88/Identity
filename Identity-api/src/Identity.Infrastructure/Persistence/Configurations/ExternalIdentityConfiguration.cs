using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations;

public sealed class ExternalIdentityConfiguration : IEntityTypeConfiguration<ExternalIdentity>
{
    public void Configure(EntityTypeBuilder<ExternalIdentity> b)
    {
        b.ToTable("external_identities");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedOnAdd();
        b.Property(x => x.Provider).HasMaxLength(30).IsRequired();
        b.Property(x => x.ProviderSubject).HasMaxLength(255).IsRequired();
        b.Property(x => x.EmailAtLinkTime).HasMaxLength(254).UseCollation("utf8mb4_unicode_ci").IsRequired();
        b.Property(x => x.DisplayName).HasMaxLength(255).IsRequired();
        b.Property(x => x.AvatarUrl).HasMaxLength(2048);
        b.Property(x => x.LastLoginDate).HasColumnType("datetime(6)").IsRequired();
        b.HasIndex(x => new { x.Provider, x.ProviderSubject })
            .IsUnique()
            .HasDatabaseName("UQExternalIdentitiesProviderSubject");
        b.HasIndex(x => new { x.UserId, x.Provider })
            .IsUnique()
            .HasDatabaseName("UQExternalIdentitiesUserProvider");
    }
}
