using MediatR;

namespace Identity.Application.Menus.DeleteMenu;

public sealed record DeleteMenuCommand(ulong Id, ulong Version, string? Actor) : IRequest;
