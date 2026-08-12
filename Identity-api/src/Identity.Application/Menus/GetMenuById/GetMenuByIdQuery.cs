using Identity.Application.Menus.Dtos;
using MediatR;

namespace Identity.Application.Menus.GetMenuById;

public sealed record GetMenuByIdQuery(ulong Id) : IRequest<MenuDto>;
