using Identity.Application.Abstractions.Persistence;
using Identity.Application.Actions.Dtos;
using Identity.Application.Common.Exceptions;
using MediatR;

namespace Identity.Application.Actions.GetActionById;

public sealed class GetActionByIdQueryHandler(IActionsReadRepository repository) : IRequestHandler<GetActionByIdQuery, ActionDto>
{
    public async Task<ActionDto> Handle(GetActionByIdQuery request, CancellationToken cancellationToken) =>
        await repository.GetByIdAsync(request.Id, cancellationToken)
        ?? throw new NotFoundException($"Action '{request.Id}' was not found.");
}

