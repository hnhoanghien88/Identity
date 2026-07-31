using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations;

public sealed class UsersConfiguration : IEntityTypeConfiguration<Users>
{
    public void Configure(EntityTypeBuilder<Users> builder)
    {
        builder.ToTable("users");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName(nameof(Users.Id)).HasColumnType("char(36)");
        builder.Property(x => x.Code).HasColumnName(nameof(Users.Code)).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Name).HasColumnName(nameof(Users.Name)).HasMaxLength(200).IsRequired();
        builder.Property(x => x.CreatedDate).HasColumnName(nameof(Users.CreatedDate)).HasColumnType("datetime(6)").IsRequired();
        builder.Property(x => x.IsActive).HasColumnName(nameof(Users.IsActive)).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique().HasDatabaseName("ux_users_code");
    }
}

