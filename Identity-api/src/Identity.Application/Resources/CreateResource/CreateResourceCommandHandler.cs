using FluentValidation;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Exceptions;
using Identity.Application.Resources.Dtos;
using MediatR;
using ResourceEntity = Identity.Domain.Entities.Resources;

namespace Identity.Application.Resources.CreateResource;

public sealed class CreateResourceCommandHandler(
    IResourcesRepository repository,
    IValidator<CreateResourceCommand> validator)
    : IRequestHandler<CreateResourceCommand, ResourceDto>
{
    public async Task<ResourceDto> Handle(
        CreateResourceCommand request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        if (!await repository.IsApplicationAvailableAsync(
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
                null,
                cancellationToken))
        {
            throw new ConflictException(
                "Code is already in use for this Application.",
                "code");
        }

        var resource = new ResourceEntity
        {
            ApplicationId = request.ApplicationId,
            Code = code,
            Name = ResourceRules.Clean(request.Name),
            ResourceType = ResourceRules.Clean(request.ResourceType),
            Description = ResourceRules.CleanDescription(request.Description),
            CreatedBy = request.Actor,
            CreatedDate = DateTime.UtcNow,
            IsActive = true,
            IsDeleted = false,
            Version = 1,
        };
        await repository.AddAsync(resource, cancellationToken);
        return ResourceRules.ToDto(resource);
    }
}