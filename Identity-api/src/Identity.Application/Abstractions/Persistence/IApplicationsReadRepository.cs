using Identity.Application.Applications.Dtos;
using Identity.Application.Applications.GetApplications;

namespace Identity.Application.Abstractions.Persistence;

public interface IApplicationsReadRepository
{
    Task<ApplicationDto?> GetByIdAsync(ulong id, CancellationToken cancellationToken);
    Task<PagedApplicationsDto> GetAsync(ApplicationsFilter filter, IReadOnlyList<ApplicationsSort> sorts, int page, int pageSize, CancellationToken cancellationToken);
}
