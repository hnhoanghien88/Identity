using Identity.Application.Resources.Dtos;
using MediatR;

namespace Identity.Application.Resources.GetResourceById;

public sealed record GetResourceByIdQuery(ulong Id) : IRequest<ResourceDto>;