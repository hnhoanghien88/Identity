using Identity.Application.Actions.Dtos;
using Identity.Application.Actions.GetActions;

namespace Identity.Application.Abstractions.Persistence;

public interface IActionsReadRepository
{
    Task<ActionDto?> GetByIdAsync(ulong id, CancellationToken cancellationToken);
    Task<PagedActionsDto> GetAsync(ActionsFilter filter, IReadOnlyList<ActionsSort> sorts, int page, int pageSize, CancellationToken cancellationToken);
}

