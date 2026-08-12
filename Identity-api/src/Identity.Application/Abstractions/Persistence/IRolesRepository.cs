using Identity.Application.Roles.Dtos;
using RoleEntity = Identity.Domain.Entities.Roles;

namespace Identity.Application.Abstractions.Persistence;

public interface IRolesRepository
{
    Task<RoleEntity?> GetByIdAsync(ulong id, CancellationToken cancellationToken);
    Task<RoleDto> GetDtoAsync(ulong id, CancellationToken cancellationToken);
    Task<bool> ApplicationExistsAsync(ulong id, CancellationToken cancellationToken);
    Task<bool> CodeExistsAsync(ulong applicationId, string code, ulong? excludingId, CancellationToken cancellationToken);
    Task<bool> HasDependenciesAsync(ulong id, CancellationToken cancellationToken);
    Task AddAsync(RoleEntity role, CancellationToken cancellationToken);
    Task SaveAsync(RoleEntity role, CancellationToken cancellationToken);
    Task DeleteAsync(RoleEntity role, CancellationToken cancellationToken);
}
