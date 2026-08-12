using Identity.Application.Resources.Dtos;
using Identity.Application.Users.GetUsers;
using MediatR;

namespace Identity.Application.Resources.GetResources;

public sealed record GetResourcesQuery(
    ResourcesFilter? Filter = null,
    IReadOnlyList<ResourcesSort>? Sorts = null,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedResourcesDto>;

public sealed record ResourcesFilter(
    ulong? ApplicationId = null,
    StringFilter? Application = null,
    StringFilter? Code = null,
    StringFilter? Name = null,
    StringFilter? ResourceType = null,
    bool? IsActive = null);

public sealed record ResourcesSort(
    ResourcesSortColumn Column,
    SortDirection Direction = SortDirection.Ascending);

public enum ResourcesSortColumn
{
    Id,
    Application,
    Code,
    Name,
    ResourceType,
    CreatedDate,
    IsActive,
}