using Identity.Application.Menus.Dtos;
using MediatR;

namespace Identity.Application.Menus.GetMenus;

public sealed record GetMenusQuery(ulong ApplicationId) : IRequest<IReadOnlyList<MenuDto>>;
