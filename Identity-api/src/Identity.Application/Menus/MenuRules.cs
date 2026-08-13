using Identity.Application.Menus.Dtos;
using MenuEntity = Identity.Domain.Entities.Menus;

namespace Identity.Application.Menus;

public static class MenuRules
{
    public static string Clean(string value) => value.Trim();
    public static string? CleanOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    public static MenuDto ToDto(MenuEntity value) =>
        new(value.Id, value.ApplicationId, value.Application.Name, value.ParentId,
            value.ResourceId, value.Resource?.Name, value.Resource?.Code, value.Code, value.Name,
            value.Route, value.Icon, value.SortOrder, value.IsVisible,
            value.IsActive, value.Version, []);
}
