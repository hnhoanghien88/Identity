using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence;

internal static class SoftDeleteCascade
{
    public static async Task SaveAsync(IdentityDbContext db, Func<Task> cascade, CancellationToken ct)
    {
        if (db.Database.CurrentTransaction is not null)
        {
            await cascade();
            await db.SaveChangesAsync(ct);
            return;
        }

        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        await cascade();
        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
    }
    public static async Task ApplicationAsync(IdentityDbContext db, ulong id, string? actor, DateTime now, CancellationToken ct)
    {
        var resourceIds = db.Resources.IgnoreQueryFilters().Where(x => x.ApplicationId == id).Select(x => x.Id);
        var roleIds = db.Roles.IgnoreQueryFilters().Where(x => x.ApplicationId == id).Select(x => x.Id);
        var permissionIds = db.Permissions.IgnoreQueryFilters().Where(x => resourceIds.Contains(x.ResourceId)).Select(x => x.Id);

        await MarkAsync(db.RolePermissions.IgnoreQueryFilters().Where(x => roleIds.Contains(x.RoleId) || permissionIds.Contains(x.PermissionId)), actor, now, ct);
        await MarkAsync(db.UserRoles.IgnoreQueryFilters().Where(x => roleIds.Contains(x.RoleId)), actor, now, ct);
        await MarkAsync(db.Permissions.IgnoreQueryFilters().Where(x => resourceIds.Contains(x.ResourceId)), actor, now, ct);
        await MarkAsync(db.RefreshTokens.IgnoreQueryFilters().Where(x => x.ApplicationId == id), actor, now, ct);
        await MarkAsync(db.RateLimitPolicies.IgnoreQueryFilters().Where(x => x.ApplicationId == id), actor, now, ct);
        await MarkAsync(db.Menus.IgnoreQueryFilters().Where(x => x.ApplicationId == id), actor, now, ct);
        await MarkAsync(db.Resources.IgnoreQueryFilters().Where(x => x.ApplicationId == id), actor, now, ct);
        await MarkAsync(db.Roles.IgnoreQueryFilters().Where(x => x.ApplicationId == id), actor, now, ct);
    }

    public static async Task ResourceAsync(IdentityDbContext db, ulong id, string? actor, DateTime now, CancellationToken ct)
    {
        var permissionIds = db.Permissions.IgnoreQueryFilters().Where(x => x.ResourceId == id).Select(x => x.Id);
        await MarkAsync(db.RolePermissions.IgnoreQueryFilters().Where(x => permissionIds.Contains(x.PermissionId)), actor, now, ct);
        await MarkAsync(db.Permissions.IgnoreQueryFilters().Where(x => x.ResourceId == id), actor, now, ct);
        await MarkAsync(db.Menus.IgnoreQueryFilters().Where(x => x.ResourceId == id), actor, now, ct);
    }

    public static async Task ActionAsync(IdentityDbContext db, ulong id, string? actor, DateTime now, CancellationToken ct)
    {
        var permissionIds = db.Permissions.IgnoreQueryFilters().Where(x => x.ActionId == id).Select(x => x.Id);
        await MarkAsync(db.RolePermissions.IgnoreQueryFilters().Where(x => permissionIds.Contains(x.PermissionId)), actor, now, ct);
        await MarkAsync(db.Permissions.IgnoreQueryFilters().Where(x => x.ActionId == id), actor, now, ct);
    }

    public static async Task RoleAsync(IdentityDbContext db, ulong id, string? actor, DateTime now, CancellationToken ct)
    {
        await MarkAsync(db.RolePermissions.IgnoreQueryFilters().Where(x => x.RoleId == id), actor, now, ct);
        await MarkAsync(db.UserRoles.IgnoreQueryFilters().Where(x => x.RoleId == id), actor, now, ct);
    }

    public static async Task UserAsync(IdentityDbContext db, ulong id, string? actor, DateTime now, CancellationToken ct)
    {
        await MarkAsync(db.UserRoles.IgnoreQueryFilters().Where(x => x.UserId == id), actor, now, ct);
        await MarkAsync(db.RefreshTokens.IgnoreQueryFilters().Where(x => x.UserId == id), actor, now, ct);
    }

    public static async Task MenuAsync(IdentityDbContext db, ulong id, string? actor, DateTime now, CancellationToken ct)
    {
        var pending = new List<ulong> { id };
        var descendants = new HashSet<ulong>();
        while (pending.Count > 0)
        {
            var parents = pending.ToArray();
            pending.Clear();
            var children = await db.Menus.IgnoreQueryFilters()
                .Where(x => x.ParentId.HasValue && parents.Contains(x.ParentId.Value))
                .Select(x => x.Id).ToListAsync(ct);
            foreach (var child in children)
                if (descendants.Add(child)) pending.Add(child);
        }
        if (descendants.Count > 0)
            await MarkAsync(db.Menus.IgnoreQueryFilters().Where(x => descendants.Contains(x.Id)), actor, now, ct);
    }

    private static Task<int> MarkAsync<TEntity>(IQueryable<TEntity> query, string? actor, DateTime now, CancellationToken ct)
        where TEntity : ActiveEntity => query.ExecuteUpdateAsync(setters => setters
            .SetProperty(x => x.IsActive, false)
            .SetProperty(x => x.IsDeleted, true)
            .SetProperty(x => x.UpdatedBy, actor)
            .SetProperty(x => x.UpdatedDate, now), ct);
}