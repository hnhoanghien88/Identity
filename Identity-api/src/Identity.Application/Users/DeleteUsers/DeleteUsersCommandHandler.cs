using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Exceptions;
using MediatR;

namespace Identity.Application.Users.DeleteUsers;

public sealed class DeleteUsersCommandHandler(
    IUsersRepository repository,
    IRefreshTokenRepository refreshTokens)
    : IRequestHandler<DeleteUsersCommand>
{
    public async Task Handle(DeleteUsersCommand request, CancellationToken ct)
    {
        if (request.Id == request.ActorId)
            throw new ConflictException();

        var user = await repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException($"User '{request.Id}' was not found.");

        if (user.Version != request.Version)
            throw new ConflictException();

        user.IsActive = false;
        user.IsDeleted = true;
        user.SecurityStamp = Guid.NewGuid();
        user.PermissionVersion++;
        user.Version++;
        user.UpdatedDate = DateTime.UtcNow;
        user.UpdatedBy = request.ActorEmail;
        await repository.DeleteAsync(user, ct);
        await refreshTokens.RevokeAllForUserAsync(user.Id, request.ActorEmail, ct);
    }
}
