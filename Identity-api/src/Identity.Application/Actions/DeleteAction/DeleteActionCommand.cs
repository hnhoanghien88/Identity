using MediatR;

namespace Identity.Application.Actions.DeleteAction;

public sealed record DeleteActionCommand(ulong Id, ulong Version) : IRequest;

