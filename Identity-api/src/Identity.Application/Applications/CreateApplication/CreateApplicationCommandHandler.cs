using Identity.Application.Abstractions.Persistence;
using Identity.Application.Applications.Dtos;
using Identity.Application.Common.Exceptions;
using MediatR;
using ApplicationEntity = Identity.Domain.Entities.Applications;

namespace Identity.Application.Applications.CreateApplication;

public sealed class CreateApplicationCommandHandler(IApplicationsRepository repository) : IRequestHandler<CreateApplicationCommand, ApplicationDto>
{
    public async Task<ApplicationDto> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
    {
        var code = ApplicationRules.Clean(request.Code);
        if (await repository.CodeExistsAsync(code, null, cancellationToken)) throw new ConflictException("Code is already in use.", "code");
        var application = new ApplicationEntity { Code = code, Name = ApplicationRules.Clean(request.Name), Audience = ApplicationRules.Clean(request.Audience), Description = ApplicationRules.CleanDescription(request.Description), CreatedBy = request.Actor, CreatedDate = DateTime.UtcNow, IsActive = true, IsDeleted = false, Version = 1 };
        await repository.AddAsync(application, cancellationToken);
        return ApplicationRules.ToDto(application);
    }
}
