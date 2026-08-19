using Identity.Application.Abstractions.Persistence;
using Identity.Application.Actions.CreateAction;
using Identity.Application.Actions.DeleteAction;
using Identity.Application.Actions.UpdateAction;
using Identity.Application.Common.Exceptions;
using ActionEntity = Identity.Domain.Entities.PermissionActions;

namespace Identity.Application.Tests.Actions;

public sealed class ActionHandlersTests
{
    [Fact]
    public async Task Create_trims_values_and_sets_version_and_audit()
    {
        var repository = new Repository();
        var result = await new CreateActionCommandHandler(repository, new CreateActionValidator()).Handle(
            new CreateActionCommand(" READ ", " Read ", "admin"),
            CancellationToken.None);

        Assert.Equal("READ", result.Code);
        Assert.Equal("Read", result.Name);
        Assert.Equal(1UL, result.Version);
        Assert.Equal("admin", repository.Value!.CreatedBy);
    }

    [Fact]
    public async Task Update_increments_version_and_rejects_stale_request()
    {
        var repository = new Repository { Value = Existing(2) };
        var handler = new UpdateActionCommandHandler(repository, new UpdateActionValidator());
        var result = await handler.Handle(
            new UpdateActionCommand(1, "WRITE", "Write", 2, "admin"),
            CancellationToken.None);

        Assert.Equal(3UL, result.Version);
        await Assert.ThrowsAsync<ConflictException>(() => handler.Handle(
            new UpdateActionCommand(1, "WRITE", "Write", 2, "admin"),
            CancellationToken.None));
    }

    [Fact]
    public async Task Delete_soft_deletes_even_when_permission_dependencies_exist()
    {
        var repository = new Repository { Value = Existing(), Dependencies = true };
        await new DeleteActionCommandHandler(repository).Handle(
            new DeleteActionCommand(1, 1, "admin"),
            CancellationToken.None);

        Assert.True(repository.Value!.IsDeleted);
        Assert.False(repository.Value.IsActive);
        Assert.Equal("admin", repository.Value.UpdatedBy);
    }

    private static ActionEntity Existing(ulong version = 1) => new()
    {
        Id = 1,
        Code = "READ",
        Name = "Read",
        CreatedDate = DateTime.UtcNow,
        Version = version,
    };

    private sealed class Repository : IActionsRepository
    {
        public ActionEntity? Value { get; set; }
        public bool Dependencies { get; set; }
        public Task<ActionEntity?> GetByIdAsync(ulong id, CancellationToken cancellationToken) => Task.FromResult(Value);
        public Task<bool> CodeExistsAsync(string code, ulong? excludingId, CancellationToken cancellationToken) => Task.FromResult(false);
        public Task<bool> HasDependenciesAsync(ulong id, CancellationToken cancellationToken) => Task.FromResult(Dependencies);
        public Task AddAsync(ActionEntity action, CancellationToken cancellationToken) { action.Id = 1; Value = action; return Task.CompletedTask; }
        public Task SaveAsync(ActionEntity action, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task DeleteAsync(ActionEntity action, CancellationToken cancellationToken) { Value = null; return Task.CompletedTask; }
    }
}

