using Identity.Application.Menus.Dtos;

namespace Identity.Application.Abstractions.Persistence;

public interface IMenusReadRepository
{
    Task<IReadOnlyList<MenuRowDto>> GetByApplicationAsync(ulong applicationId, CancellationToken cancellationToken);
    Task<MenuRowDto?> GetByIdAsync(ulong id, CancellationToken cancellationToken);
}
