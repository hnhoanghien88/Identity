using Identity.Application.Actions.Dtos;
using Identity.Application.Users.GetUsers;
using MediatR;

namespace Identity.Application.Actions.GetActions;

public sealed record GetActionsQuery(ActionsFilter? Filter = null, IReadOnlyList<ActionsSort>? Sorts = null, int Page = 1, int PageSize = 20) : IRequest<PagedActionsDto>;
public sealed record ActionsFilter(StringFilter? Code = null, StringFilter? Name = null);
public sealed record ActionsSort(ActionsSortColumn Column, SortDirection Direction = SortDirection.Ascending);
public enum ActionsSortColumn { Id, Code, Name, CreatedDate }

