using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations;

public sealed class UsersConfiguration : IEntityTypeConfiguration<Users>
{
    public void Configure(EntityTypeBuilder<Users> b)
    {
        b.ToTable("users");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedOnAdd();
        b.Property(x => x.Code).HasMaxLength(50).UseCollation("utf8mb4_unicode_ci").IsRequired();
        b.Property(x => x.Email).HasMaxLength(254).UseCollation("utf8mb4_unicode_ci").IsRequired();
        b.Property(x => x.DisplayName).HasMaxLength(255).IsRequired();
        b.Property(x => x.PasswordHash).HasMaxLength(500);
        b.Property(x => x.SecurityStamp).HasColumnType("char(36)").IsRequired();
        b.Property(x => x.PermissionVersion).HasDefaultValue(1);
        b.Property(x => x.Version).HasDefaultValue(1UL).IsConcurrencyToken().IsRequired();
        b.Property(x => x.CreatedDate).HasColumnType("datetime(6)").HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
        b.Property(x => x.UpdatedDate).HasColumnType("datetime(6)");
        b.HasIndex(x => x.Code).IsUnique().HasDatabaseName("UQUsersCode");
        b.HasIndex(x => x.Email).IsUnique().HasDatabaseName("UQUsersEmail");
    }
}
