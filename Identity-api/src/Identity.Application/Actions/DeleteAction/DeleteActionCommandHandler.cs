using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Exceptions;
using MediatR;

namespace Identity.Application.Actions.DeleteAction;

public sealed class DeleteActionCommandHandler(IActionsRepository repository) : IRequestHandler<DeleteActionCommand>
{
    public async Task Handle(DeleteActionCommand request, CancellationToken cancellationToken)
    {
        var action = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Action '{request.Id}' was not found.");
        if (action.Version != request.Version)
            throw new ConflictException("The Action was changed by another user. Reload and try again.");
        action.IsActive = false;
        action.IsDeleted = true;
        action.UpdatedBy = request.Actor;
        action.UpdatedDate = DateTime.UtcNow;
        action.Version++;
        await repository.SaveAsync(action, cancellationToken);
    }
}