using Identity.Application.Abstractions.Persistence;
using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using RolePermissionEntity = Identity.Domain.Entities.RolePermissions;

namespace Identity.Infrastructure.IntegrationTests.RolePermissions;

public sealed class RolePermissionsPersistenceContractTests
{
    [Fact]
    public void Repository_implements_application_contract() =>
        Assert.True(typeof(IRolePermissionsRepository).IsAssignableFrom(
            typeof(MySqlRolePermissionsRepository)));

    [Fact]
    public void Existing_unique_boundaries_prevent_duplicate_grants()
    {
        var builder = new ModelBuilder();
        new PermissionsConfiguration().Configure(builder.Entity<Permissions>());
        new RolePermissionsConfiguration().Configure(builder.Entity<RolePermissionEntity>());

        var permission = builder.Model.FindEntityType(typeof(Permissions))!;
        var rolePermission = builder.Model.FindEntityType(typeof(RolePermissionEntity))!;
        Assert.Null(permission.FindProperty("IsDeleted"));
        Assert.Null(permission.FindProperty("IsActive"));
        Assert.Contains(permission.GetIndexes(), index =>
            index.IsUnique && index.Properties.Select(value => value.Name)
                .SequenceEqual([nameof(Permissions.ResourceId), nameof(Permissions.ActionId)]));
        Assert.Contains(rolePermission.GetIndexes(), index =>
            index.IsUnique && index.Properties.Select(value => value.Name)
                .SequenceEqual([nameof(RolePermissionEntity.RoleId), nameof(RolePermissionEntity.PermissionId)]));
    }
}


