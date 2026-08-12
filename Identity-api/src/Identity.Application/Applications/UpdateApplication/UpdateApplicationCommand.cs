using Identity.Application.Applications.Dtos;
using MediatR;

namespace Identity.Application.Applications.UpdateApplication;

public sealed record UpdateApplicationCommand(ulong Id, string Code, string Name, string Audience, string? Description, bool IsActive, ulong Version, string? Actor) : IRequest<ApplicationDto>;
