using Identity.Application.Abstractions.Persistence;
using Identity.Application.Resources.Dtos;
using Identity.Application.Users.GetUsers;
using MediatR;

namespace Identity.Application.Resources.GetResources;

public sealed class GetResourcesQueryHandler(IResourcesReadRepository repository)
    : IRequestHandler<GetResourcesQuery, PagedResourcesDto>
{
    public Task<PagedResourcesDto> Handle(
        GetResourcesQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(request.Page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(request.PageSize, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(request.PageSize, 100);
        var sorts = request.Sorts is { Count: > 0 }
            ? request.Sorts
            : [
                new ResourcesSort(
                    ResourcesSortColumn.CreatedDate,
                    SortDirection.Descending),
                new ResourcesSort(
                    ResourcesSortColumn.Id,
                    SortDirection.Descending),
            ];
        return repository.GetAsync(
            request.Filter ?? new ResourcesFilter(),
            sorts,
            request.Page,
            request.PageSize,
            cancellationToken);
    }
}