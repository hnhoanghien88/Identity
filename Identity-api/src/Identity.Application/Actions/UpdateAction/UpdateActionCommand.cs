using Identity.Application.Actions.Dtos;
using MediatR;

namespace Identity.Application.Actions.UpdateAction;

public sealed record UpdateActionCommand(ulong Id, string Code, string Name, ulong Version, string? Actor) : IRequest<ActionDto>;

