using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Exceptions;
using Identity.Application.RolePermissions;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence;

public sealed class MySqlRolePermissionsRepository(IdentityDbContext db)
    : IRolePermissionsRepository
{
    public async Task<RolePermissionSnapshotDto> GetSnapshotAsync(
        ulong roleId,
        ulong resourceId,
        CancellationToken cancellationToken)
    {
        await EnsureRoleAsync(roleId, cancellationToken);
        await EnsureResourceAsync(resourceId, cancellationToken);
        await EnsureSameApplicationAsync(roleId, resourceId, cancellationToken);
        var actions = await db.PermissionActions
            .AsNoTracking()
            .OrderBy(action => action.Code)
            .ThenBy(action => action.Id)
            .Select(action => new ActionGrantDto(
                action.Id,
                action.Code,
                action.Name,
                db.RolePermissions.Any(rolePermission =>
                    rolePermission.RoleId == roleId
                    && rolePermission.Permission.ResourceId == resourceId
                    && rolePermission.Permission.ActionId == action.Id)))
            .ToListAsync(cancellationToken);
        return new RolePermissionSnapshotDto(roleId, resourceId, actions);
    }

    public async Task GrantAsync(
        ulong roleId,
        ulong resourceId,
        ulong actionId,
        string? actor,
        CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        await EnsureRoleAsync(roleId, cancellationToken);
        var resource = await GetResourceAsync(resourceId, cancellationToken);
        await EnsureSameApplicationAsync(roleId, resourceId, cancellationToken);
        var action = await GetActionAsync(actionId, cancellationToken);
        var permission = await db.Permissions.SingleOrDefaultAsync(
            value => value.ResourceId == resourceId && value.ActionId == actionId,
            cancellationToken);
        if (permission is null)
        {
            permission = new Permissions
            {
                ResourceId = resourceId,
                ActionId = actionId,
                Code = $"{resource.Code}.{action.Code}",
                Name = action.Name,
                CreatedBy = actor,
                CreatedDate = DateTime.UtcNow,
            };
            db.Permissions.Add(permission);
            await SaveChangesAsync(cancellationToken);
        }
        if (!await db.RolePermissions.AnyAsync(
                value => value.RoleId == roleId && value.PermissionId == permission.Id,
                cancellationToken))
        {
            db.RolePermissions.Add(new RolePermissions
            {
                RoleId = roleId,
                PermissionId = permission.Id,
                CreatedBy = actor,
                CreatedDate = DateTime.UtcNow,
            });
            await SaveChangesAsync(cancellationToken);
            await IncrementPermissionVersionsAsync(roleId, cancellationToken);
        }
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task RevokeAsync(
        ulong roleId,
        ulong resourceId,
        ulong actionId,
        string? actor,
        CancellationToken cancellationToken)
    {
        await EnsureRoleAsync(roleId, cancellationToken);
        await EnsureResourceAsync(resourceId, cancellationToken);
        await EnsureSameApplicationAsync(roleId, resourceId, cancellationToken);
        await EnsureActionAsync(actionId, cancellationToken);
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var permission = await db.Permissions.SingleOrDefaultAsync(
            value => value.ResourceId == resourceId && value.ActionId == actionId,
            cancellationToken);
        if (permission is null)
            return;

        var rolePermission = await db.RolePermissions.SingleOrDefaultAsync(
            value => value.RoleId == roleId && value.PermissionId == permission.Id,
            cancellationToken);
        if (rolePermission is null)
            return;

        rolePermission.UpdatedBy = actor;
        rolePermission.UpdatedDate = DateTime.UtcNow;
        db.RolePermissions.Remove(rolePermission);
        await SaveChangesAsync(cancellationToken);
        await IncrementPermissionVersionsAsync(roleId, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private Task IncrementPermissionVersionsAsync(
        ulong roleId,
        CancellationToken cancellationToken) =>
        db.Users
            .Where(user => db.UserRoles.Any(userRole =>
                userRole.RoleId == roleId
                && userRole.UserId == user.Id
                && userRole.IsActive))
            .ExecuteUpdateAsync(
                updates => updates.SetProperty(
                    user => user.PermissionVersion,
                    user => user.PermissionVersion + 1),
                cancellationToken);
    private async Task EnsureRoleAsync(ulong id, CancellationToken cancellationToken)
    {
        if (!await db.Roles.AnyAsync(
                role => role.Id == id && role.IsActive && !role.IsDeleted,
                cancellationToken))
            throw new NotFoundException($"Role '{id}' was not found.");
    }

    private async Task EnsureResourceAsync(ulong id, CancellationToken cancellationToken) =>
        _ = await GetResourceAsync(id, cancellationToken);

    private async Task EnsureSameApplicationAsync(
        ulong roleId,
        ulong resourceId,
        CancellationToken cancellationToken)
    {
        var isSameApplication = await db.Roles
            .Where(role => role.Id == roleId)
            .AnyAsync(
                role => db.Resources.Any(resource =>
                    resource.Id == resourceId
                    && resource.ApplicationId == role.ApplicationId),
                cancellationToken);

        if (!isSameApplication)
            throw new ConflictException(
                "Role and Resource must belong to the same Application.");
    }

    private async Task<Resources> GetResourceAsync(ulong id, CancellationToken cancellationToken) =>
        await db.Resources.SingleOrDefaultAsync(
            resource => resource.Id == id && resource.IsActive && !resource.IsDeleted,
            cancellationToken)
        ?? throw new NotFoundException($"Resource '{id}' was not found.");

    private async Task EnsureActionAsync(ulong id, CancellationToken cancellationToken) =>
        _ = await GetActionAsync(id, cancellationToken);

    private async Task<PermissionActions> GetActionAsync(
        ulong id,
        CancellationToken cancellationToken) =>
        await db.PermissionActions.SingleOrDefaultAsync(
            action => action.Id == id,
            cancellationToken)
        ?? throw new NotFoundException($"Action '{id}' was not found.");

    private async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            throw new ConflictException(
                "Role Permission data changed concurrently. Reload and try again.");
        }
    }
}


