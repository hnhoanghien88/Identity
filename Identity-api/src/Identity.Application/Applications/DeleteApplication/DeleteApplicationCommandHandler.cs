using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Exceptions;
using MediatR;

namespace Identity.Application.Applications.DeleteApplication;

public sealed class DeleteApplicationCommandHandler(IApplicationsRepository repository) : IRequestHandler<DeleteApplicationCommand>
{
    public async Task Handle(DeleteApplicationCommand request, CancellationToken cancellationToken)
    {
        var application = await repository.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException($"Application '{request.Id}' was not found.");
        if (application.Version != request.Version) throw new ConflictException("The Application was changed by another user. Reload and try again.");
        application.IsActive = false;
        application.IsDeleted = true;
        application.UpdatedBy = request.Actor;
        application.UpdatedDate = DateTime.UtcNow;
        application.Version++;
        await repository.SaveAsync(application, cancellationToken);
    }
}
