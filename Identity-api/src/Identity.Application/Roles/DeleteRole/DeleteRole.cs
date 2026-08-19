using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Exceptions;
using MediatR;

namespace Identity.Application.Roles.DeleteRole;

public sealed record DeleteRoleCommand(ulong Id, ulong Version, string? Actor = null) : IRequest;

public sealed class DeleteRoleCommandHandler(IRolesRepository repository) : IRequestHandler<DeleteRoleCommand>
{
    public async Task Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Role '{request.Id}' was not found.");
        if (role.Version != request.Version)
            throw new ConflictException("The Role was changed by another user. Reload and try again.");
        if (role.IsSystemRole)
            throw new ConflictException("System Roles cannot be deleted.");
        role.IsActive = false;
        role.IsDeleted = true;
        role.UpdatedBy = request.Actor;
        role.UpdatedDate = DateTime.UtcNow;
        role.Version++;
        await repository.SaveAsync(role, cancellationToken);
    }
}