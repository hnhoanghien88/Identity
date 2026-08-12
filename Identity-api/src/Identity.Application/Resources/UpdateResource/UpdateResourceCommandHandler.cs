using FluentValidation;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Exceptions;
using Identity.Application.Resources.Dtos;
using MediatR;

namespace Identity.Application.Resources.UpdateResource;

public sealed class UpdateResourceCommandHandler(
    IResourcesRepository repository,
    IValidator<UpdateResourceCommand> validator)
    : IRequestHandler<UpdateResourceCommand, ResourceDto>
{
    public async Task<ResourceDto> Handle(
        UpdateResourceCommand request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var resource = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Resource '{request.Id}' was not found.");
        if (resource.Version != request.Version)
            throw new ConflictException("The Resource was changed by another user. Reload and try again.");
        if (resource.ApplicationId != request.ApplicationId
            && !await repository.IsApplicationAvailableAsync(
                request.ApplicationId,
                cancellationToken))
        {
            throw new ConflictException(
                "The selected Application is unavailable.",
                "applicationId");
        }

        var code = ResourceRules.Clean(request.Code);
        if (await repository.CodeExistsAsync(
                request.ApplicationId,
                code,
                request.Id,
                cancellationToken))
        {
            throw new ConflictException(
                "Code is already in use for this Application.",
                "code");
        }

        resource.ApplicationId = request.ApplicationId;
        resource.Code = code;
        resource.Name = ResourceRules.Clean(request.Name);
        resource.ResourceType = ResourceRules.Clean(request.ResourceType);
        resource.Description = ResourceRules.CleanDescription(request.Description);
        resource.IsActive = request.IsActive;
        resource.UpdatedBy = request.Actor;
        resource.UpdatedDate = DateTime.UtcNow;
        resource.Version++;
        await repository.SaveAsync(resource, cancellationToken);
        return ResourceRules.ToDto(resource);
    }
}