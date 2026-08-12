using Identity.Application.RolePermissions;

namespace Identity.Application.Abstractions.Persistence;

public interface IRolePermissionsRepository
{
    Task<RolePermissionSnapshotDto> GetSnapshotAsync(
        ulong roleId,
        ulong resourceId,
        CancellationToken cancellationToken);

    Task GrantAsync(
        ulong roleId,
        ulong resourceId,
        ulong actionId,
        string? actor,
        CancellationToken cancellationToken);

    Task RevokeAsync(
        ulong roleId,
        ulong resourceId,
        ulong actionId,
        string? actor,
        CancellationToken cancellationToken);
}
