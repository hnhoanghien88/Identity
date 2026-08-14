using Identity.Application.Abstractions.Persistence;
using Identity.Application.Menus.Dtos;
using Identity.Application.Menus.GetMenus;

namespace Identity.Application.Tests.Menus;

public sealed class GetMenusTests
{
    [Fact]
    public async Task Builds_ordered_recursive_tree_and_omits_invalid_nodes()
    {
        var rows = new List<MenuRowDto>
        {
            Row(2, 1, 2, "Child"),
            Row(1, null, 1, "Root"),
            Row(3, 99, 0, "Orphan"),
            Row(4, 5, 0, "Cycle A"),
            Row(5, 4, 0, "Cycle B"),
        };
        var handler = new GetMenusQueryHandler(new Stub(rows));

        var result = await handler.Handle(new GetMenusQuery(10), CancellationToken.None);

        var root = Assert.Single(result);
        Assert.Equal((ulong)1, root.Id);
        Assert.Equal((ulong)2, Assert.Single(root.Children).Id);
    }

    private static MenuRowDto Row(ulong id, ulong? parentId, int order, string name) =>
        new(id, 10, "App", parentId, null, null, null, $"M{id}", name, null, null,
            order, true, true, 1);

    private sealed class Stub(IReadOnlyList<MenuRowDto> rows) : IMenusReadRepository
    {
        public Task<IReadOnlyList<MenuRowDto>> GetByApplicationAsync(ulong applicationId, CancellationToken cancellationToken) =>
            Task.FromResult(rows);
        public Task<MenuRowDto?> GetByIdAsync(ulong id, CancellationToken cancellationToken) =>
            Task.FromResult(rows.FirstOrDefault(x => x.Id == id));
    }
}
