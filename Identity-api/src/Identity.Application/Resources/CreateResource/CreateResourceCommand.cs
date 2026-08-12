using Identity.Application.Resources.Dtos;
using MediatR;

namespace Identity.Application.Resources.CreateResource;

public sealed record CreateResourceCommand(
    ulong ApplicationId,
    string Code,
    string Name,
    string ResourceType,
    string? Description,
    string? Actor) : IRequest<ResourceDto>;