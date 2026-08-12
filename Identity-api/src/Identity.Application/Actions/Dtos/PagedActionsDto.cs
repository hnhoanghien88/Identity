namespace Identity.Application.Actions.Dtos;

public sealed record PagedActionsDto(IReadOnlyList<ActionDto> Items, int TotalCount, int Page, int PageSize);

