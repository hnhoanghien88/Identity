using MediatR;

namespace Identity.Application.Resources.DeleteResource;

public sealed record DeleteResourceCommand(
    ulong Id,
    ulong Version,
    string? Actor) : IRequest;