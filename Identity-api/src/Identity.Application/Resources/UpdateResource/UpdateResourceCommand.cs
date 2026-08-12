using Identity.Application.Resources.Dtos;
using MediatR;

namespace Identity.Application.Resources.UpdateResource;

public sealed record UpdateResourceCommand(
    ulong Id,
    ulong ApplicationId,
    string Code,
    string Name,
    string ResourceType,
    string? Description,
    bool IsActive,
    ulong Version,
    string? Actor) : IRequest<ResourceDto>;