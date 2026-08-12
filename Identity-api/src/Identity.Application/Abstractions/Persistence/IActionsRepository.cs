using ActionEntity = Identity.Domain.Entities.PermissionActions;

namespace Identity.Application.Abstractions.Persistence;

public interface IActionsRepository
{
    Task<ActionEntity?> GetByIdAsync(ulong id, CancellationToken cancellationToken);
    Task<bool> CodeExistsAsync(string code, ulong? excludingId, CancellationToken cancellationToken);
    Task<bool> HasDependenciesAsync(ulong id, CancellationToken cancellationToken);
    Task AddAsync(ActionEntity action, CancellationToken cancellationToken);
    Task SaveAsync(ActionEntity action, CancellationToken cancellationToken);
    Task DeleteAsync(ActionEntity action, CancellationToken cancellationToken);
}

