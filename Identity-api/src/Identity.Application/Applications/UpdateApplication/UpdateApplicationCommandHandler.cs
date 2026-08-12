using FluentValidation;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Applications.Dtos;
using Identity.Application.Common.Exceptions;
using MediatR;

namespace Identity.Application.Applications.UpdateApplication;

public sealed class UpdateApplicationCommandHandler(IApplicationsRepository repository, IValidator<UpdateApplicationCommand> validator) : IRequestHandler<UpdateApplicationCommand, ApplicationDto>
{
    public async Task<ApplicationDto> Handle(UpdateApplicationCommand request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var application = await repository.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException($"Application '{request.Id}' was not found.");
        if (application.Version != request.Version) throw new ConflictException("The Application was changed by another user. Reload and try again.");
        var code = ApplicationRules.Clean(request.Code);
        if (await repository.CodeExistsAsync(code, request.Id, cancellationToken)) throw new ConflictException("Code is already in use.", "code");
        application.Code = code;
        application.Name = ApplicationRules.Clean(request.Name);
        application.Audience = ApplicationRules.Clean(request.Audience);
        application.Description = ApplicationRules.CleanDescription(request.Description);
        application.IsActive = request.IsActive;
        application.UpdatedBy = request.Actor;
        application.UpdatedDate = DateTime.UtcNow;
        application.Version++;
        await repository.SaveAsync(application, cancellationToken);
        return ApplicationRules.ToDto(application);
    }
}
