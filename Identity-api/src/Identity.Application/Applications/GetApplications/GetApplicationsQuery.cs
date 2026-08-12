using Identity.Application.Applications.Dtos;
using Identity.Application.Users.GetUsers;
using MediatR;

namespace Identity.Application.Applications.GetApplications;

public sealed record GetApplicationsQuery(ApplicationsFilter? Filter = null, IReadOnlyList<ApplicationsSort>? Sorts = null, int Page = 1, int PageSize = 20) : IRequest<PagedApplicationsDto>;
public sealed record ApplicationsFilter(StringFilter? Code = null, StringFilter? Name = null, StringFilter? Audience = null, bool? IsActive = null);
public sealed record ApplicationsSort(ApplicationsSortColumn Column, SortDirection Direction = SortDirection.Ascending);
public enum ApplicationsSortColumn { Id, Code, Name, Audience, CreatedDate, IsActive }
