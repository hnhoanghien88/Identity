using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Exceptions;
using MediatR;

namespace Identity.Application.Users.ActivateUsers;

public sealed class ActivateUsersCommandHandler(IUsersRepository repository)
    : IRequestHandler<ActivateUsersCommand>
{
    public async Task Handle(ActivateUsersCommand request, CancellationToken ct)
    {
        var user = await repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException($"User '{request.Id}' was not found.");

        user.IsActive = request.IsActive;
        await repository.UpdateAsync(user, ct);
    }
}
