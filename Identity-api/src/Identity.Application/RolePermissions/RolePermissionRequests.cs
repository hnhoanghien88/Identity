using Identity.Application.Abstractions.Persistence;
using MediatR;

namespace Identity.Application.RolePermissions;

public sealed record ActionGrantDto(
    ulong ActionId,
    string Code,
    string Name,
    bool IsGranted);

public sealed record RolePermissionSnapshotDto(
    ulong RoleId,
    ulong ResourceId,
    IReadOnlyList<ActionGrantDto> Actions);

public sealed record GetRolePermissionSnapshotQuery(
    ulong RoleId,
    ulong ResourceId) : IRequest<RolePermissionSnapshotDto>;

public sealed class GetRolePermissionSnapshotQueryHandler(IRolePermissionsRepository repository)
    : IRequestHandler<GetRolePermissionSnapshotQuery, RolePermissionSnapshotDto>
{
    public Task<RolePermissionSnapshotDto> Handle(
        GetRolePermissionSnapshotQuery request,
        CancellationToken cancellationToken)
    {
        Validate(request.RoleId, request.ResourceId);
        return repository.GetSnapshotAsync(
            request.RoleId,
            request.ResourceId,
            cancellationToken);
    }

    private static void Validate(ulong roleId, ulong resourceId)
    {
        ArgumentOutOfRangeException.ThrowIfZero(roleId);
        ArgumentOutOfRangeException.ThrowIfZero(resourceId);
    }
}

public sealed record GrantRolePermissionCommand(
    ulong RoleId,
    ulong ResourceId,
    ulong ActionId,
    string? Actor) : IRequest;

public sealed class GrantRolePermissionCommandHandler(IRolePermissionsRepository repository)
    : IRequestHandler<GrantRolePermissionCommand>
{
    public async Task Handle(
        GrantRolePermissionCommand request,
        CancellationToken cancellationToken)
    {
        Validate(request.RoleId, request.ResourceId, request.ActionId);
        await repository.GrantAsync(
            request.RoleId,
            request.ResourceId,
            request.ActionId,
            request.Actor,
            cancellationToken);
    }

    private static void Validate(ulong roleId, ulong resourceId, ulong actionId)
    {
        ArgumentOutOfRangeException.ThrowIfZero(roleId);
        ArgumentOutOfRangeException.ThrowIfZero(resourceId);
        ArgumentOutOfRangeException.ThrowIfZero(actionId);
    }
}

public sealed record RevokeRolePermissionCommand(
    ulong RoleId,
    ulong ResourceId,
    ulong ActionId,
    string? Actor) : IRequest;

public sealed class RevokeRolePermissionCommandHandler(IRolePermissionsRepository repository)
    : IRequestHandler<RevokeRolePermissionCommand>
{
    public async Task Handle(
        RevokeRolePermissionCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfZero(request.RoleId);
        ArgumentOutOfRangeException.ThrowIfZero(request.ResourceId);
        ArgumentOutOfRangeException.ThrowIfZero(request.ActionId);
        await repository.RevokeAsync(
            request.RoleId,
            request.ResourceId,
            request.ActionId,
            request.Actor,
            cancellationToken);
    }
}
