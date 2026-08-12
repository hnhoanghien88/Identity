using FluentValidation;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Actions.Dtos;
using Identity.Application.Common.Exceptions;
using MediatR;

namespace Identity.Application.Actions.UpdateAction;

public sealed class UpdateActionCommandHandler(IActionsRepository repository, IValidator<UpdateActionCommand> validator) : IRequestHandler<UpdateActionCommand, ActionDto>
{
    public async Task<ActionDto> Handle(UpdateActionCommand request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var action = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Action '{request.Id}' was not found.");
        if (action.Version != request.Version)
            throw new ConflictException("The Action was changed by another user. Reload and try again.");
        var code = ActionRules.Clean(request.Code);
        if (await repository.CodeExistsAsync(code, request.Id, cancellationToken))
            throw new ConflictException("Code is already in use.", "code");
        action.Code = code;
        action.Name = ActionRules.Clean(request.Name);
        action.UpdatedBy = request.Actor;
        action.UpdatedDate = DateTime.UtcNow;
        action.Version++;
        await repository.SaveAsync(action, cancellationToken);
        return ActionRules.ToDto(action);
    }
}

