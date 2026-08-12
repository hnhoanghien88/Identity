using Identity.Application.Menus.Dtos;
using MediatR;

namespace Identity.Application.Menus.CreateMenu;

public sealed record CreateMenuCommand(ulong ApplicationId, ulong? ParentId, ulong? ResourceId,
    string Code, string Name, string? Route, string? Icon, int SortOrder,
    bool IsVisible, bool IsActive, string? Actor) : IRequest<MenuDto>;
