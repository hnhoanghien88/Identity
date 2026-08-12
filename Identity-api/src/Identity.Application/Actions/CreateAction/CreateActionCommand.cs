using Identity.Application.Actions.Dtos;
using MediatR;

namespace Identity.Application.Actions.CreateAction;

public sealed record CreateActionCommand(string Code, string Name, string? Actor) : IRequest<ActionDto>;

