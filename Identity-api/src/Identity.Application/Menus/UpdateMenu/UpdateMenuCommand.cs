using Identity.Application.Menus.Dtos;
using MediatR;

namespace Identity.Application.Menus.UpdateMenu;

public sealed record UpdateMenuCommand(ulong Id, ulong ApplicationId, ulong? ParentId, ulong? ResourceId,
    string Code, string Name, string? Route, string? Icon, int SortOrder, bool IsVisible,
    bool IsActive, ulong Version, string? Actor) : IRequest<MenuDto>;
