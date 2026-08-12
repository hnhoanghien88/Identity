using FluentValidation;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Applications.CreateApplication;
using Identity.Application.Applications.DeleteApplication;
using Identity.Application.Applications.UpdateApplication;
using Identity.Application.Common.Exceptions;
using ApplicationEntity = Identity.Domain.Entities.Applications;

namespace Identity.Application.Tests.Applications;

public sealed class ApplicationCommandTests
{
    [Fact]
    public async Task Create_trims_values_and_sets_audit_and_version()
    {
        var repository = new FakeApplicationsRepository();
        var handler = new CreateApplicationCommandHandler(
            repository,
            new CreateApplicationValidator());

        var result = await handler.Handle(
            new CreateApplicationCommand(
                " APP ",
                " Name ",
                " Audience ",
                " Description ",
                "admin@example.test"),
            CancellationToken.None);

        Assert.Equal("APP", result.Code);
        Assert.Equal("Name", result.Name);
        Assert.Equal("Audience", result.Audience);
        Assert.Equal("Description", result.Description);
        Assert.True(result.IsActive);
        Assert.Equal(1UL, result.Version);
        Assert.Equal("admin@example.test", repository.Value!.CreatedBy);
    }

    [Fact]
    public async Task Create_rejects_duplicate_code()
    {
        var repository = new FakeApplicationsRepository { Duplicate = true };
        var handler = new CreateApplicationCommandHandler(
            repository,
            new CreateApplicationValidator());

        var error = await Assert.ThrowsAsync<ConflictException>(() =>
            handler.Handle(
                new CreateApplicationCommand("APP", "Name", "Audience", null, null),
                CancellationToken.None));

        Assert.Equal("code", error.Field);
        Assert.Null(repository.Value);
    }

    [Fact]
    public async Task Create_rejects_whitespace_required_values()
    {
        var handler = new CreateApplicationCommandHandler(
            new FakeApplicationsRepository(),
            new CreateApplicationValidator());

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(
                new CreateApplicationCommand(" ", "Name", "Audience", null, null),
                CancellationToken.None));
    }

    [Fact]
    public async Task Update_rejects_stale_version_without_saving()
    {
        var repository = new FakeApplicationsRepository
        {
            Value = Existing(version: 2),
        };
        var handler = new UpdateApplicationCommandHandler(
            repository,
            new UpdateApplicationValidator());

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.Handle(
                new UpdateApplicationCommand(
                    1,
                    "APP",
                    "Changed",
                    "Audience",
                    null,
                    true,
                    1,
                    "admin"),
                CancellationToken.None));

        Assert.Equal(0, repository.SaveCount);
        Assert.Equal("Name", repository.Value.Name);
    }

    [Fact]
    public async Task Update_increments_version_once_and_stamps_audit()
    {
        var repository = new FakeApplicationsRepository
        {
            Value = Existing(version: 3),
        };
        var handler = new UpdateApplicationCommandHandler(
            repository,
            new UpdateApplicationValidator());

        var result = await handler.Handle(
            new UpdateApplicationCommand(
                1,
                "APP",
                "Changed",
                "New audience",
                "",
                false,
                3,
                "admin"),
            CancellationToken.None);

        Assert.Equal(4UL, result.Version);
        Assert.False(result.IsActive);
        Assert.Null(result.Description);
        Assert.Equal("admin", repository.Value!.UpdatedBy);
        Assert.Equal(1, repository.SaveCount);
    }

    [Fact]
    public async Task Delete_is_blocked_when_dependencies_exist()
    {
        var repository = new FakeApplicationsRepository
        {
            Value = Existing(),
            DependenciesExist = true,
        };
        var handler = new DeleteApplicationCommandHandler(repository);

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.Handle(
                new DeleteApplicationCommand(1, 1, "admin"),
                CancellationToken.None));

        Assert.False(repository.Value.IsDeleted);
        Assert.Equal(0, repository.SaveCount);
    }

    [Fact]
    public async Task Delete_soft_deletes_and_increments_version()
    {
        var repository = new FakeApplicationsRepository
        {
            Value = Existing(),
        };
        var handler = new DeleteApplicationCommandHandler(repository);

        await handler.Handle(
            new DeleteApplicationCommand(1, 1, "admin"),
            CancellationToken.None);

        Assert.True(repository.Value.IsDeleted);
        Assert.False(repository.Value.IsActive);
        Assert.Equal(2UL, repository.Value.Version);
        Assert.Equal("admin", repository.Value.UpdatedBy);
        Assert.Equal(1, repository.SaveCount);
    }

    private static ApplicationEntity Existing(ulong version = 1) =>
        new()
        {
            Id = 1,
            Code = "APP",
            Name = "Name",
            Audience = "Audience",
            CreatedDate = DateTime.UtcNow,
            IsActive = true,
            Version = version,
        };

    private sealed class FakeApplicationsRepository : IApplicationsRepository
    {
        public ApplicationEntity? Value { get; set; }
        public bool Duplicate { get; set; }
        public bool DependenciesExist { get; set; }
        public int SaveCount { get; private set; }

        public Task<ApplicationEntity?> GetByIdAsync(
            ulong id,
            CancellationToken cancellationToken) =>
            Task.FromResult(Value is { IsDeleted: false } ? Value : null);

        public Task<bool> CodeExistsAsync(
            string code,
            ulong? excludingId,
            CancellationToken cancellationToken) =>
            Task.FromResult(Duplicate);

        public Task<bool> HasDependenciesAsync(
            ulong id,
            CancellationToken cancellationToken) =>
            Task.FromResult(DependenciesExist);

        public Task AddAsync(
            ApplicationEntity application,
            CancellationToken cancellationToken)
        {
            application.Id = 1;
            Value = application;
            return Task.CompletedTask;
        }

        public Task SaveAsync(
            ApplicationEntity application,
            CancellationToken cancellationToken)
        {
            SaveCount++;
            return Task.CompletedTask;
        }
    }
}

