using Identity.Application.Abstractions.Persistence;
using Identity.Application.Actions.Dtos;
using Identity.Application.Users.GetUsers;
using MediatR;

namespace Identity.Application.Actions.GetActions;

public sealed class GetActionsQueryHandler(IActionsReadRepository repository) : IRequestHandler<GetActionsQuery, PagedActionsDto>
{
    public Task<PagedActionsDto> Handle(GetActionsQuery request, CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(request.Page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(request.PageSize, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(request.PageSize, 100);
        var sorts = request.Sorts is { Count: > 0 }
            ? request.Sorts
            : [
                new ActionsSort(ActionsSortColumn.CreatedDate, SortDirection.Descending),
                new ActionsSort(ActionsSortColumn.Id, SortDirection.Descending),
            ];
        return repository.GetAsync(request.Filter ?? new ActionsFilter(), sorts, request.Page, request.PageSize, cancellationToken);
    }
}

