using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using MenuEntity = Identity.Domain.Entities.Menus;

namespace Identity.Infrastructure.Persistence;

public sealed class MySqlMenusRepository(IdentityDbContext db) : IMenusRepository
{
    public Task<MenuEntity?> GetByIdAsync(ulong id, CancellationToken cancellationToken) =>
        db.Menus.Include(x => x.Application).Include(x => x.Resource)
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    public Task<bool> IsApplicationAvailableAsync(ulong id, CancellationToken cancellationToken) =>
        db.Applications.AnyAsync(x => x.Id == id && x.IsActive && !x.IsDeleted, cancellationToken);
    public Task<bool> IsResourceValidAsync(ulong applicationId, ulong resourceId, CancellationToken cancellationToken) =>
        db.Resources.AnyAsync(x => x.Id == resourceId && x.ApplicationId == applicationId && x.IsActive && !x.IsDeleted, cancellationToken);
    public Task<bool> CodeExistsAsync(ulong applicationId, string code, ulong? excludingId, CancellationToken cancellationToken) =>
        db.Menus.AnyAsync(x => x.Id != excludingId && x.ApplicationId == applicationId && x.Code == code, cancellationToken);
    public Task<bool> HasChildrenAsync(ulong id, CancellationToken cancellationToken) =>
        db.Menus.AnyAsync(x => x.ParentId == id && !x.IsDeleted, cancellationToken);

    public async Task<bool> IsParentValidAsync(ulong applicationId, ulong parentId, ulong? movingId, CancellationToken cancellationToken)
    {
        var visited = new HashSet<ulong>();
        ulong? currentId = parentId;
        while (currentId.HasValue)
        {
            if (!visited.Add(currentId.Value) || currentId == movingId) return false;
            var current = await db.Menus.AsNoTracking().Where(x => x.Id == currentId && !x.IsDeleted)
                .Select(x => new { x.ApplicationId, x.ParentId }).SingleOrDefaultAsync(cancellationToken);
            if (current is null || current.ApplicationId != applicationId) return false;
            currentId = current.ParentId;
        }
        return true;
    }

    public async Task AddAsync(MenuEntity menu, CancellationToken cancellationToken)
    {
        await LoadRelationsAsync(menu, cancellationToken);
        db.Menus.Add(menu);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task SaveAsync(MenuEntity menu, CancellationToken cancellationToken)
    {
        await LoadRelationsAsync(menu, cancellationToken);
        if (menu.IsDeleted)
            await SoftDeleteCascade.SaveAsync(db, () => SoftDeleteCascade.MenuAsync(
                db, menu.Id, menu.UpdatedBy, menu.UpdatedDate ?? DateTime.UtcNow, cancellationToken), cancellationToken);
        else
            await SaveChangesAsync(cancellationToken);
    }

    private async Task LoadRelationsAsync(MenuEntity menu, CancellationToken cancellationToken)
    {
        menu.Application = await db.Applications.SingleAsync(x => x.Id == menu.ApplicationId, cancellationToken);
        menu.Resource = menu.ResourceId.HasValue
            ? await db.Resources.SingleAsync(x => x.Id == menu.ResourceId, cancellationToken)
            : null;
    }

    private async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try { await db.SaveChangesAsync(cancellationToken); }
        catch (DbUpdateConcurrencyException) { throw new ConflictException("The Menu was changed by another user. Reload and try again."); }
        catch (DbUpdateException exception)
        {
            var detail = exception.InnerException?.Message ?? exception.Message;
            if (detail.Contains("UQMenus", StringComparison.OrdinalIgnoreCase)
                || detail.Contains("menus_applicationid_code_unique", StringComparison.OrdinalIgnoreCase))
                throw new ConflictException("Code is already in use for this Application.", "code");
            throw new ConflictException("Menu data conflicts with an existing record.");
        }
    }
}
