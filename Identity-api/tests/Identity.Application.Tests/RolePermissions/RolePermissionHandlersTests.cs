using Identity.Application.Abstractions.Persistence;
using Identity.Application.RolePermissions;

namespace Identity.Application.Tests.RolePermissions;

public sealed class RolePermissionHandlersTests
{
    [Fact]
    public async Task Snapshot_handler_delegates_selected_pair()
    {
        var repository = new Repository();
        var result = await new GetRolePermissionSnapshotQueryHandler(repository).Handle(
            new GetRolePermissionSnapshotQuery(10, 20),
            CancellationToken.None);

        Assert.Equal(10UL, result.RoleId);
        Assert.Equal(20UL, result.ResourceId);
        Assert.True(Assert.Single(result.Actions).IsGranted);
    }

    [Fact]
    public async Task Grant_handler_passes_actor_and_ids()
    {
        var repository = new Repository();
        await new GrantRolePermissionCommandHandler(repository).Handle(
            new GrantRolePermissionCommand(10, 20, 30, "tester"),
            CancellationToken.None);

        Assert.Equal((10UL, 20UL, 30UL, "tester"), repository.Granted);
    }

    [Fact]
    public async Task Revoke_handler_passes_actor_and_ids()
    {
        var repository = new Repository();
        await new RevokeRolePermissionCommandHandler(repository).Handle(
            new RevokeRolePermissionCommand(10, 20, 30, "tester"),
            CancellationToken.None);

        Assert.Equal((10UL, 20UL, 30UL, "tester"), repository.Revoked);
    }

    [Fact]
    public async Task Handlers_reject_zero_identifiers()
    {
        var repository = new Repository();
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            new GetRolePermissionSnapshotQueryHandler(repository).Handle(
                new GetRolePermissionSnapshotQuery(0, 20),
                CancellationToken.None));
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            new GrantRolePermissionCommandHandler(repository).Handle(
                new GrantRolePermissionCommand(10, 20, 0, null),
                CancellationToken.None));
    }

    private sealed class Repository : IRolePermissionsRepository
    {
        public (ulong, ulong, ulong, string?)? Granted { get; private set; }
        public (ulong, ulong, ulong, string?)? Revoked { get; private set; }

        public Task<RolePermissionSnapshotDto> GetSnapshotAsync(
            ulong roleId,
            ulong resourceId,
            CancellationToken cancellationToken) =>
            Task.FromResult(new RolePermissionSnapshotDto(
                roleId,
                resourceId,
                [new ActionGrantDto(30, "VIEW", "View", true)]));

        public Task GrantAsync(
            ulong roleId,
            ulong resourceId,
            ulong actionId,
            string? actor,
            CancellationToken cancellationToken)
        {
            Granted = (roleId, resourceId, actionId, actor);
            return Task.CompletedTask;
        }

        public Task RevokeAsync(
            ulong roleId,
            ulong resourceId,
            ulong actionId,
            string? actor,
            CancellationToken cancellationToken)
        {
            Revoked = (roleId, resourceId, actionId, actor);
            return Task.CompletedTask;
        }
    }
}
