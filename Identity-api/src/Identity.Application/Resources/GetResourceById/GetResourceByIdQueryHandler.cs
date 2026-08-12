using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Exceptions;
using Identity.Application.Resources.Dtos;
using MediatR;

namespace Identity.Application.Resources.GetResourceById;

public sealed class GetResourceByIdQueryHandler(IResourcesReadRepository repository)
    : IRequestHandler<GetResourceByIdQuery, ResourceDto>
{
    public async Task<ResourceDto> Handle(
        GetResourceByIdQuery request,
        CancellationToken cancellationToken) =>
        await repository.GetByIdAsync(request.Id, cancellationToken)
        ?? throw new NotFoundException($"Resource '{request.Id}' was not found.");
}