using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations;

public sealed class ApplicationsConfiguration : IEntityTypeConfiguration<Applications>
{
    public void Configure(EntityTypeBuilder<Applications> b)
    {
        b.ToTable("applications");
        b.Property(x => x.Code).HasMaxLength(50);
        b.Property(x => x.Name).HasMaxLength(150);
        b.Property(x => x.Audience).HasMaxLength(150);
        b.Property(x => x.Description).HasMaxLength(500);
        b.HasIndex(x => x.Code).IsUnique().HasDatabaseName("UQApplicationsCode");
    }
}

public sealed class RolesConfiguration : IEntityTypeConfiguration<Roles>
{
    public void Configure(EntityTypeBuilder<Roles> b)
    {
        b.ToTable("roles");
        b.Property(x => x.Code).HasMaxLength(100);
        b.Property(x => x.Name).HasMaxLength(150);
        b.HasIndex(x => new { x.ApplicationId, x.Code }).IsUnique().HasDatabaseName("UQRoles");
    }
}

public sealed class ResourcesConfiguration : IEntityTypeConfiguration<Resources>
{
    public void Configure(EntityTypeBuilder<Resources> b)
    {
        b.ToTable("resources");
        b.Property(x => x.Code).HasMaxLength(120);
        b.Property(x => x.Name).HasMaxLength(150);
        b.Property(x => x.ResourceType).HasMaxLength(30);
        b.Property(x => x.Description).HasMaxLength(500);
        b.HasIndex(x => new { x.ApplicationId, x.Code }).IsUnique().HasDatabaseName("UQResources");
    }
}

public sealed class PermissionActionsConfiguration : IEntityTypeConfiguration<PermissionActions>
{
    public void Configure(EntityTypeBuilder<PermissionActions> b)
    {
        b.ToTable("permission_actions");
        b.Property(x => x.Code).HasMaxLength(50);
        b.Property(x => x.Name).HasMaxLength(100);
        b.HasIndex(x => x.Code).IsUnique().HasDatabaseName("UQPermissionActions");
    }
}

public sealed class PermissionsConfiguration : IEntityTypeConfiguration<Permissions>
{
    public void Configure(EntityTypeBuilder<Permissions> b)
    {
        b.ToTable("permissions");
        b.Property(x => x.Code).HasMaxLength(200);
        b.Property(x => x.Name).HasMaxLength(150);
        b.HasIndex(x => new { x.ResourceId, x.ActionId }).IsUnique().HasDatabaseName("UQPermissions");
    }
}

public sealed class UserRolesConfiguration : IEntityTypeConfiguration<UserRoles>
{
    public void Configure(EntityTypeBuilder<UserRoles> b)
    {
        b.ToTable("user_roles");
        b.Property(x => x.IsActive).HasDefaultValue(true);
        b.HasIndex(x => new { x.UserId, x.RoleId }).IsUnique().HasDatabaseName("UQUserRoles");
    }
}

public sealed class RolePermissionsConfiguration : IEntityTypeConfiguration<RolePermissions>
{
    public void Configure(EntityTypeBuilder<RolePermissions> b)
    {
        b.ToTable("role_permissions");
        b.HasIndex(x => new { x.RoleId, x.PermissionId }).IsUnique().HasDatabaseName("UQRolePermissions");
    }
}

public sealed class MenusConfiguration : IEntityTypeConfiguration<Menus>
{
    public void Configure(EntityTypeBuilder<Menus> b)
    {
        b.ToTable("menus");
        b.Property(x => x.Code).HasMaxLength(120);
        b.Property(x => x.Name).HasMaxLength(150);
        b.Property(x => x.Route).HasMaxLength(300);
        b.Property(x => x.Icon).HasMaxLength(100);
        b.Property(x => x.SortOrder).HasDefaultValue(0);
        b.Property(x => x.IsVisible).HasDefaultValue(true);
        b.HasIndex(x => new { x.ApplicationId, x.Code }).IsUnique().HasDatabaseName("UQMenus");
    }
}

public sealed class RefreshTokensConfiguration : IEntityTypeConfiguration<RefreshTokens>
{
    public void Configure(EntityTypeBuilder<RefreshTokens> b)
    {
        b.ToTable("refresh_tokens");
        b.Property(x => x.TokenHash).HasMaxLength(64).IsFixedLength();
        b.Property(x => x.JwtId).HasColumnType("char(36)");
        b.Property(x => x.FamilyId).HasColumnType("char(36)");
        b.Property(x => x.ExpiresDate).HasColumnType("datetime(6)");
        b.Property(x => x.RevokedDate).HasColumnType("datetime(6)");
        b.Property(x => x.IsActive).HasDefaultValue(true);
        b.HasIndex(x => x.TokenHash).IsUnique().HasDatabaseName("UQRefreshTokenHash");
    }
}