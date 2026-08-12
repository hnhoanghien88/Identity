using ApplicationEntity = Identity.Domain.Entities.Applications;

namespace Identity.Application.Abstractions.Persistence;

public interface IApplicationsRepository
{
    Task<ApplicationEntity?> GetByIdAsync(ulong id, CancellationToken cancellationToken);
    Task<bool> CodeExistsAsync(string code, ulong? excludingId, CancellationToken cancellationToken);
    Task<bool> HasDependenciesAsync(ulong id, CancellationToken cancellationToken);
    Task AddAsync(ApplicationEntity application, CancellationToken cancellationToken);
    Task SaveAsync(ApplicationEntity application, CancellationToken cancellationToken);
}
