using Identity.Application.Actions.Dtos;
using MediatR;

namespace Identity.Application.Actions.GetActionById;

public sealed record GetActionByIdQuery(ulong Id) : IRequest<ActionDto>;

