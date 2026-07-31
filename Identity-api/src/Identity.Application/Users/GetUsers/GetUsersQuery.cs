using Identity.Application.Users.Dtos;
using MediatR;

namespace Identity.Application.Users.GetUsers;

public sealed record GetUsersQuery(
    UsersFilter? Filter = null,
    IReadOnlyList<UsersSort>? Sorts = null,
    int Page = 1,
    int PageSize = 20) : IRequest<IReadOnlyList<UsersDto>>;

public sealed record UsersFilter(
    IReadOnlyCollection<Guid>? Ids = null,
    StringFilter? Code = null,
    StringFilter? Name = null,
    DateTime? CreatedDateFrom = null,
    DateTime? CreatedDateTo = null,
    bool? IsActive = null);

public sealed record UsersSort(UsersSortColumn Column, SortDirection Direction = SortDirection.Ascending);

public enum UsersSortColumn
{
    Id,
    Code,
    Name,
    CreatedDate,
    IsActive
}

public enum SortDirection
{
    Ascending,
    Descending
}

