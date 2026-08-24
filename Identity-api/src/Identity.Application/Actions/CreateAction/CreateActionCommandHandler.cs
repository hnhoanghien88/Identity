using Identity.Application.Abstractions.Persistence;
using Identity.Application.Actions.Dtos;
using Identity.Application.Common.Exceptions;
using MediatR;
using ActionEntity = Identity.Domain.Entities.PermissionActions;

namespace Identity.Application.Actions.CreateAction;

public sealed class CreateActionCommandHandler(IActionsRepository repository) : IRequestHandler<CreateActionCommand, ActionDto>
{
    public async Task<ActionDto> Handle(CreateActionCommand request, CancellationToken cancellationToken)
    {
        var code = ActionRules.Clean(request.Code);
        if (await repository.CodeExistsAsync(code, null, cancellationToken))
            throw new ConflictException("Code is already in use.", "code");
        var action = new ActionEntity
        {
            Code = code,
            Name = ActionRules.Clean(request.Name),
            CreatedBy = request.Actor,
            CreatedDate = DateTime.UtcNow,
            Version = 1,
        };
        await repository.AddAsync(action, cancellationToken);
        return ActionRules.ToDto(action);
    }
}

