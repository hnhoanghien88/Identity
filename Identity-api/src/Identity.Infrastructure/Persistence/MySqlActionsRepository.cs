using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using ActionEntity = Identity.Domain.Entities.PermissionActions;

namespace Identity.Infrastructure.Persistence;

public sealed class MySqlActionsRepository(IdentityDbContext db) : IActionsRepository
{
    public Task<ActionEntity?> GetByIdAsync(ulong id, CancellationToken cancellationToken) =>
        db.PermissionActions.SingleOrDefaultAsync(action => action.Id == id, cancellationToken);

    public Task<bool> CodeExistsAsync(string code, ulong? excludingId, CancellationToken cancellationToken) =>
        db.PermissionActions.AnyAsync(action => action.Id != excludingId && action.Code == code, cancellationToken);

    public Task<bool> HasDependenciesAsync(ulong id, CancellationToken cancellationToken) =>
        db.Permissions.AnyAsync(permission => permission.ActionId == id, cancellationToken);

    public async Task AddAsync(ActionEntity action, CancellationToken cancellationToken)
    {
        db.PermissionActions.Add(action);
        await SaveChangesAsync(cancellationToken);
    }

    public Task SaveAsync(ActionEntity action, CancellationToken cancellationToken) => SaveChangesAsync(cancellationToken);

    public async Task DeleteAsync(ActionEntity action, CancellationToken cancellationToken)
    {
        db.PermissionActions.Remove(action);
        await SaveChangesAsync(cancellationToken);
    }

    private async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException("The Action was changed by another user. Reload and try again.");
        }
        catch (DbUpdateException exception)
        {
            var detail = exception.InnerException?.Message ?? exception.Message;
            if (detail.Contains("UQPermissionActions", StringComparison.OrdinalIgnoreCase)
                || detail.Contains("permission_actions_code_unique", StringComparison.OrdinalIgnoreCase))
                throw new ConflictException("Code is already in use.", "code");
            if (detail.Contains("FK_permissions_permission_actions_ActionId", StringComparison.OrdinalIgnoreCase))
                throw new ConflictException("This Action cannot be deleted while related Permissions exist.");
            throw new ConflictException("Action data conflicts with an existing record.");
        }
    }
}

