using Identity.Application.Applications.Dtos;
using MediatR;

namespace Identity.Application.Applications.CreateApplication;

public sealed record CreateApplicationCommand(string Code, string Name, string Audience, string? Description, string? Actor) : IRequest<ApplicationDto>;
