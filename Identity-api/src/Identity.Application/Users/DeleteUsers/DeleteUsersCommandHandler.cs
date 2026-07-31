using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Exceptions;
using MediatR;

namespace Identity.Application.Users.DeleteUsers;

public sealed class DeleteUsersCommandHandler(IUsersRepository repository)
    : IRequestHandler<DeleteUsersCommand>
{
    public async Task Handle(DeleteUsersCommand request, CancellationToken ct)
    {
        var user = await repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException($"User '{request.Id}' was not found.");

        await repository.DeleteAsync(user, ct);
    }
}
