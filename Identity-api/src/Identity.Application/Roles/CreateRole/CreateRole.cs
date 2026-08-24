using FluentValidation;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Exceptions;
using Identity.Application.Roles.Dtos;
using MediatR;
using RoleEntity = Identity.Domain.Entities.Roles;

namespace Identity.Application.Roles.CreateRole;

public sealed record CreateRoleCommand(
    ulong ApplicationId,
    string Code,
    string Name,
    bool IsSystemRole,
    bool IsActive,
    string? Actor) : IRequest<RoleDto>;

public sealed class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleValidator()
    {
        RuleFor(x => x.ApplicationId).GreaterThan(0UL);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
    }
}

public sealed class CreateRoleCommandHandler(IRolesRepository repository)
    : IRequestHandler<CreateRoleCommand, RoleDto>
{
    public async Task<RoleDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        if (!await repository.ApplicationExistsAsync(request.ApplicationId, cancellationToken))
            throw new ConflictException("The selected Application is unavailable.", "applicationId");
        var code = RoleRules.Clean(request.Code);
        if (await repository.CodeExistsAsync(request.ApplicationId, code, null, cancellationToken))
            throw new ConflictException("Code is already in use for this Application.", "code");
        var role = new RoleEntity
        {
            ApplicationId = request.ApplicationId,
            Code = code,
            Name = RoleRules.Clean(request.Name),
            IsSystemRole = request.IsSystemRole,
            IsActive = request.IsActive,
            IsDeleted = false,
            CreatedBy = request.Actor,
            CreatedDate = DateTime.UtcNow,
            Version = 1,
        };
        await repository.AddAsync(role, cancellationToken);
        return await repository.GetDtoAsync(role.Id, cancellationToken);
    }
}
