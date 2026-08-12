using MediatR;

namespace Identity.Application.Applications.DeleteApplication;

public sealed record DeleteApplicationCommand(ulong Id, ulong Version, string? Actor) : IRequest;
