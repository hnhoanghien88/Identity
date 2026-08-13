using Identity.Application.Abstractions.Persistence;
using Identity.Application.Menus.Dtos;
using MediatR;

namespace Identity.Application.Menus.GetMenus;

public sealed class GetMenusQueryHandler(IMenusReadRepository repository) : IRequestHandler<GetMenusQuery, IReadOnlyList<MenuDto>>
{
    public async Task<IReadOnlyList<MenuDto>> Handle(GetMenusQuery request, CancellationToken cancellationToken)
    {
        var rows = await repository.GetByApplicationAsync(request.ApplicationId, cancellationToken);
        var valid = RemoveInvalidHierarchy(rows);
        var roots = valid.Where(x => !x.ParentId.HasValue).ToList();
        var children = valid.Where(x => x.ParentId.HasValue)
            .GroupBy(x => x.ParentId!.Value).ToDictionary(x => x.Key, x => x.ToList());
        MenuDto Map(MenuRowDto row) => new(row.Id, row.ApplicationId, row.ApplicationName, row.ParentId,
            row.ResourceId, row.ResourceName, row.ResourceCode, row.Code, row.Name, row.Route, row.Icon, row.SortOrder,
            row.IsVisible, row.IsActive, row.Version,
            children.TryGetValue(row.Id, out var values) ? values.Select(Map).ToList() : []);
        return roots.Select(Map).ToList();
    }

    private static IReadOnlyList<MenuRowDto> RemoveInvalidHierarchy(IReadOnlyList<MenuRowDto> rows)
    {
        var byId = rows.ToDictionary(x => x.Id);
        return rows.Where(row =>
        {
            var visited = new HashSet<ulong> { row.Id };
            var parentId = row.ParentId;
            while (parentId.HasValue)
            {
                if (!visited.Add(parentId.Value) || !byId.TryGetValue(parentId.Value, out var parent)) return false;
                parentId = parent.ParentId;
            }
            return true;
        }).ToList();
    }
}

