using FluentValidation;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Exceptions;
using Identity.Application.Roles.Dtos;
using MediatR;

namespace Identity.Application.Roles.UpdateRole;

public sealed record UpdateRoleCommand(
    ulong Id,
    ulong ApplicationId,
    string Code,
    string Name,
    bool IsSystemRole,
    bool IsActive,
    ulong Version,
    string? Actor) : IRequest<RoleDto>;

public sealed class UpdateRoleValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0UL);
        RuleFor(x => x.ApplicationId).GreaterThan(0UL);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Version).GreaterThan(0UL);
    }
}

public sealed class UpdateRoleCommandHandler(IRolesRepository repository, IValidator<UpdateRoleCommand> validator)
    : IRequestHandler<UpdateRoleCommand, RoleDto>
{
    public async Task<RoleDto> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var role = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Role '{request.Id}' was not found.");
        if (role.Version != request.Version)
            throw new ConflictException("The Role was changed by another user. Reload and try again.");
        if (!await repository.ApplicationExistsAsync(request.ApplicationId, cancellationToken))
            throw new ConflictException("The selected Application is unavailable.", "applicationId");
        var code = RoleRules.Clean(request.Code);
        if (await repository.CodeExistsAsync(request.ApplicationId, code, request.Id, cancellationToken))
            throw new ConflictException("Code is already in use for this Application.", "code");
        role.ApplicationId = request.ApplicationId;
        role.Code = code;
        role.Name = RoleRules.Clean(request.Name);
        role.IsSystemRole = request.IsSystemRole;
        role.IsActive = request.IsActive;
        role.UpdatedBy = request.Actor;
        role.UpdatedDate = DateTime.UtcNow;
        role.Version++;
        await repository.SaveAsync(role, cancellationToken);
        return await repository.GetDtoAsync(role.Id, cancellationToken);
    }
}
