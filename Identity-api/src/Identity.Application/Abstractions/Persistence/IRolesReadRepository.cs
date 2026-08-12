using Identity.Application.Roles.Dtos;
using Identity.Application.Roles.GetRoles;

namespace Identity.Application.Abstractions.Persistence;

public interface IRolesReadRepository
{
    Task<RoleDto?> GetByIdAsync(ulong id, CancellationToken cancellationToken);
    Task<PagedRolesDto> GetAsync(
        RolesFilter filter,
        IReadOnlyList<RolesSort> sorts,
        int page,
        int pageSize,
        CancellationToken cancellationToken);
}
