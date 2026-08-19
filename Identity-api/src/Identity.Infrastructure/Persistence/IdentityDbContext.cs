using System.Linq.Expressions;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence;

public sealed class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    public DbSet<Users> Users => Set<Users>();
    public DbSet<Applications> Applications => Set<Applications>();
    public DbSet<Resources> Resources => Set<Resources>();
    public DbSet<PermissionActions> PermissionActions => Set<PermissionActions>();
    public DbSet<Permissions> Permissions => Set<Permissions>();
    public DbSet<Menus> Menus => Set<Menus>();
    public DbSet<Roles> Roles => Set<Roles>();
    public DbSet<UserRoles> UserRoles => Set<UserRoles>();
    public DbSet<RolePermissions> RolePermissions => Set<RolePermissions>();
    public DbSet<RefreshTokens> RefreshTokens => Set<RefreshTokens>();
    public DbSet<RateLimitPolicy> RateLimitPolicies => Set<RateLimitPolicy>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.FindProperty(nameof(EntityBase.CreatedBy))?.SetMaxLength(255);
            entity.FindProperty(nameof(EntityBase.UpdatedBy))?.SetMaxLength(255);
            entity.FindProperty(nameof(EntityBase.CreatedDate))?.SetColumnType("datetime(6)");
            entity.FindProperty(nameof(EntityBase.CreatedDate))?.SetDefaultValueSql("CURRENT_TIMESTAMP(6)");
            entity.FindProperty(nameof(EntityBase.UpdatedDate))?.SetColumnType("datetime(6)");
            entity.FindProperty(nameof(ActiveEntity.IsActive))?.SetDefaultValue(true);
            entity.FindProperty(nameof(ActiveEntity.IsDeleted))?.SetDefaultValue(false);
            if (typeof(ActiveEntity).IsAssignableFrom(entity.ClrType))
            {
                var parameter = Expression.Parameter(entity.ClrType, "entity");
                var property = Expression.Property(parameter, nameof(ActiveEntity.IsDeleted));
                entity.SetQueryFilter(Expression.Lambda(Expression.Not(property), parameter));
            }
        }
        foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(x => x.GetForeignKeys()))
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
    }
}