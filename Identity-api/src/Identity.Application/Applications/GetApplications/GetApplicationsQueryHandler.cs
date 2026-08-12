using Identity.Application.Abstractions.Persistence;
using Identity.Application.Applications.Dtos;
using Identity.Application.Users.GetUsers;
using MediatR;

namespace Identity.Application.Applications.GetApplications;

public sealed class GetApplicationsQueryHandler(IApplicationsReadRepository repository) : IRequestHandler<GetApplicationsQuery, PagedApplicationsDto>
{
    public Task<PagedApplicationsDto> Handle(GetApplicationsQuery request, CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(request.Page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(request.PageSize, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(request.PageSize, 100);
        var sorts = request.Sorts is { Count: > 0 } ? request.Sorts : [new ApplicationsSort(ApplicationsSortColumn.CreatedDate, SortDirection.Descending), new ApplicationsSort(ApplicationsSortColumn.Id, SortDirection.Descending)];
        return repository.GetAsync(request.Filter ?? new ApplicationsFilter(), sorts, request.Page, request.PageSize, cancellationToken);
    }
}
