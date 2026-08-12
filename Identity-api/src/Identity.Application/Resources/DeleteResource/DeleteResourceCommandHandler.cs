using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Exceptions;
using MediatR;

namespace Identity.Application.Resources.DeleteResource;

public sealed class DeleteResourceCommandHandler(IResourcesRepository repository)
    : IRequestHandler<DeleteResourceCommand>
{
    public async Task Handle(
        DeleteResourceCommand request,
        CancellationToken cancellationToken)
    {
        var resource = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Resource '{request.Id}' was not found.");
        if (resource.Version != request.Version)
            throw new ConflictException("The Resource was changed by another user. Reload and try again.");
        if (await repository.HasDependenciesAsync(request.Id, cancellationToken))
            throw new ConflictException("This Resource cannot be deleted while related data exists.");

        resource.IsActive = false;
        resource.IsDeleted = true;
        resource.UpdatedBy = request.Actor;
        resource.UpdatedDate = DateTime.UtcNow;
        resource.Version++;
        await repository.SaveAsync(resource, cancellationToken);
    }
}