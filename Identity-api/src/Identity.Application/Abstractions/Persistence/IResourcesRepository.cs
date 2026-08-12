using ResourceEntity = Identity.Domain.Entities.Resources;

namespace Identity.Application.Abstractions.Persistence;

public interface IResourcesRepository
{
    Task<ResourceEntity?> GetByIdAsync(ulong id, CancellationToken cancellationToken);
    Task<bool> IsApplicationAvailableAsync(ulong id, CancellationToken cancellationToken);
    Task<bool> CodeExistsAsync(
        ulong applicationId,
        string code,
        ulong? excludingId,
        CancellationToken cancellationToken);
    Task<bool> HasDependenciesAsync(ulong id, CancellationToken cancellationToken);
    Task AddAsync(ResourceEntity resource, CancellationToken cancellationToken);
    Task SaveAsync(ResourceEntity resource, CancellationToken cancellationToken);
}