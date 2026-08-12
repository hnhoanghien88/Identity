using MenuEntity = Identity.Domain.Entities.Menus;

namespace Identity.Application.Abstractions.Persistence;

public interface IMenusRepository
{
    Task<MenuEntity?> GetByIdAsync(ulong id, CancellationToken cancellationToken);
    Task<bool> IsApplicationAvailableAsync(ulong id, CancellationToken cancellationToken);
    Task<bool> IsParentValidAsync(ulong applicationId, ulong parentId, ulong? movingId, CancellationToken cancellationToken);
    Task<bool> IsResourceValidAsync(ulong applicationId, ulong resourceId, CancellationToken cancellationToken);
    Task<bool> CodeExistsAsync(ulong applicationId, string code, ulong? excludingId, CancellationToken cancellationToken);
    Task<bool> HasChildrenAsync(ulong id, CancellationToken cancellationToken);
    Task AddAsync(MenuEntity menu, CancellationToken cancellationToken);
    Task SaveAsync(MenuEntity menu, CancellationToken cancellationToken);
}
