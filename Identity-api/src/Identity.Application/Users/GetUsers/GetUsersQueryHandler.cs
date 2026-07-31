using Identity.Application.Abstractions.Persistence;
using Identity.Application.Users.Dtos;
using MediatR;

namespace Identity.Application.Users.GetUsers;

public sealed class GetUsersQueryHandler(IUsersReadRepository repository)
    : IRequestHandler<GetUsersQuery, IReadOnlyList<UsersDto>>
{
    public async Task<IReadOnlyList<UsersDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(request.Page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(request.PageSize, 1);

        var sorts = request.Sorts is { Count: > 0 }
            ? request.Sorts
            : [new UsersSort(UsersSortColumn.CreatedDate, SortDirection.Descending)];

        var users = await repository.GetAsync(
            request.Filter ?? new UsersFilter(),
            sorts,
            request.Page,
            request.PageSize,
            cancellationToken);

        return users;
    }
}
