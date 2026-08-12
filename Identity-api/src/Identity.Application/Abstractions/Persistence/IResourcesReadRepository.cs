using Identity.Application.Resources.Dtos;
using Identity.Application.Resources.GetResources;

namespace Identity.Application.Abstractions.Persistence;

public interface IResourcesReadRepository
{
    Task<ResourceDto?> GetByIdAsync(ulong id, CancellationToken cancellationToken);
    Task<PagedResourcesDto> GetAsync(
        ResourcesFilter filter,
        IReadOnlyList<ResourcesSort> sorts,
        int page,
        int pageSize,
        CancellationToken cancellationToken);
}