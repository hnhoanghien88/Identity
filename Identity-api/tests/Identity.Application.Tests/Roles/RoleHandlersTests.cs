using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Exceptions;
using Identity.Application.Roles.CreateRole;
using Identity.Application.Roles.DeleteRole;
using Identity.Application.Roles.Dtos;
using Identity.Application.Roles.UpdateRole;
using RoleEntity = Identity.Domain.Entities.Roles;

namespace Identity.Application.Tests.Roles;

public sealed class RoleHandlersTests
{
    [Fact]
    public async Task Create_trims_and_defaults_are_preserved()
    {
        var repository = new Repository();
        var result = await new CreateRoleCommandHandler(repository, new CreateRoleValidator()).Handle(
            new CreateRoleCommand(10, " ADMIN ", " Administrators ", false, true, "tester"),
            CancellationToken.None);
        Assert.Equal("ADMIN", result.Code);
        Assert.True(result.IsActive);
        Assert.Equal(1UL, result.Version);
    }

    [Fact]
    public async Task Update_rejects_stale_version()
    {
        var repository = new Repository { Value = Existing(version: 2) };
        var handler = new UpdateRoleCommandHandler(repository, new UpdateRoleValidator());
        await Assert.ThrowsAsync<ConflictException>(() => handler.Handle(
            new UpdateRoleCommand(1, 10, "ADMIN", "Admin", false, true, 1, "tester"),
            CancellationToken.None));
    }

    [Fact]
    public async Task Delete_rejects_system_role()
    {
        var repository = new Repository { Value = Existing(isSystem: true) };
        await Assert.ThrowsAsync<ConflictException>(() =>
            new DeleteRoleCommandHandler(repository).Handle(new DeleteRoleCommand(1, 1), CancellationToken.None));
    }

    private static RoleEntity Existing(ulong version = 1, bool isSystem = false) => new()
    {
        Id = 1,
        ApplicationId = 10,
        Code = "ADMIN",
        Name = "Admin",
        IsSystemRole = isSystem,
        IsActive = true,
        CreatedDate = DateTime.UtcNow,
        Version = version,
    };

    private sealed class Repository : IRolesRepository
    {
        public RoleEntity? Value { get; set; }
        public Task<RoleEntity?> GetByIdAsync(ulong id, CancellationToken cancellationToken) => Task.FromResult(Value);
        public Task<RoleDto> GetDtoAsync(ulong id, CancellationToken cancellationToken) => Task.FromResult(new RoleDto(id, 10, "APP", "Application", Value!.Code, Value.Name, Value.IsSystemRole, Value.IsActive, Value.CreatedDate, Value.Version));
        public Task<bool> ApplicationExistsAsync(ulong id, CancellationToken cancellationToken) => Task.FromResult(true);
        public Task<bool> CodeExistsAsync(ulong applicationId, string code, ulong? excludingId, CancellationToken cancellationToken) => Task.FromResult(false);
        public Task<bool> HasDependenciesAsync(ulong id, CancellationToken cancellationToken) => Task.FromResult(false);
        public Task AddAsync(RoleEntity role, CancellationToken cancellationToken) { role.Id = 1; Value = role; return Task.CompletedTask; }
        public Task SaveAsync(RoleEntity role, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task DeleteAsync(RoleEntity role, CancellationToken cancellationToken) { Value = null; return Task.CompletedTask; }
    }
}
