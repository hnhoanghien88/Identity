using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Exceptions;
using Identity.Application.Menus.UpdateMenu;
using Identity.Domain.Entities;
using MenuEntity = Identity.Domain.Entities.Menus;

namespace Identity.Application.Tests.Menus;

public sealed class UpdateMenuTests
{
    [Fact]
    public async Task Moves_leaf_menu_to_an_available_application()
    {
        var repository = new StubMenusRepository
        {
            Value = ExistingMenu(),
        };
        var handler = new UpdateMenuCommandHandler(repository);

        var result = await handler.Handle(Command(applicationId: 2), CancellationToken.None);

        Assert.Equal((ulong)2, result.ApplicationId);
        Assert.Equal((ulong)2, repository.Value!.ApplicationId);
        Assert.Equal(1, repository.SaveCount);
    }

    [Fact]
    public async Task Rejects_application_change_when_menu_has_children()
    {
        var repository = new StubMenusRepository
        {
            Value = ExistingMenu(),
            HasChildren = true,
        };
        var handler = new UpdateMenuCommandHandler(repository);

        var exception = await Assert.ThrowsAsync<ConflictException>(() =>
            handler.Handle(Command(applicationId: 2), CancellationToken.None));

        Assert.Contains("children", exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Equal((ulong)1, repository.Value!.ApplicationId);
        Assert.Equal(0, repository.SaveCount);
    }

    private static UpdateMenuCommand Command(ulong applicationId) =>
        new(
            1,
            applicationId,
            null,
            null,
            "MENU",
            "Menu",
            null,
            null,
            0,
            true,
            true,
            1,
            "tester");

    private static MenuEntity ExistingMenu() =>
        new()
        {
            Id = 1,
            ApplicationId = 1,
            Application = Application(1),
            Code = "MENU",
            Name = "Menu",
            IsActive = true,
            IsVisible = true,
            Version = 1,
        };

    private static Identity.Domain.Entities.Applications Application(ulong id) =>
        new()
        {
            Id = id,
            Code = $"APP{id}",
            Name = $"Application {id}",
            Audience = $"app-{id}",
            IsActive = true,
            Version = 1,
        };

    private sealed class StubMenusRepository : IMenusRepository
    {
        public MenuEntity? Value { get; init; }
        public bool HasChildren { get; init; }
        public int SaveCount { get; private set; }

        public Task<MenuEntity?> GetByIdAsync(ulong id, CancellationToken cancellationToken) =>
            Task.FromResult(Value);

        public Task<bool> IsApplicationAvailableAsync(
            ulong id,
            CancellationToken cancellationToken) =>
            Task.FromResult(true);

        public Task<bool> IsParentValidAsync(
            ulong applicationId,
            ulong parentId,
            ulong? movingId,
            CancellationToken cancellationToken) =>
            Task.FromResult(true);

        public Task<bool> IsResourceValidAsync(
            ulong applicationId,
            ulong resourceId,
            CancellationToken cancellationToken) =>
            Task.FromResult(true);

        public Task<bool> CodeExistsAsync(
            ulong applicationId,
            string code,
            ulong? excludingId,
            CancellationToken cancellationToken) =>
            Task.FromResult(false);

        public Task<bool> HasChildrenAsync(ulong id, CancellationToken cancellationToken) =>
            Task.FromResult(HasChildren);

        public Task AddAsync(MenuEntity menu, CancellationToken cancellationToken) =>
            Task.CompletedTask;

        public Task SaveAsync(MenuEntity menu, CancellationToken cancellationToken)
        {
            menu.Application = Application(menu.ApplicationId);
            SaveCount++;
            return Task.CompletedTask;
        }
    }
}
