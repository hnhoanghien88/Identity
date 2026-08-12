using Identity.Application.Applications.Dtos;
using MediatR;

namespace Identity.Application.Applications.GetApplicationById;

public sealed record GetApplicationByIdQuery(ulong Id) : IRequest<ApplicationDto>;
