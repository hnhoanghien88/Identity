namespace Identity.Application.Menus.Dtos;

public sealed record MenuDto(
    ulong Id,
    ulong ApplicationId,
    string ApplicationName,
    ulong? ParentId,
    ulong? ResourceId,
    string? ResourceName,
    string? ResourceCode,
    string Code,
    string Name,
    string? Route,
    string? Icon,
    int SortOrder,
    bool IsVisible,
    bool IsActive,
    ulong Version,
    IReadOnlyList<MenuDto> Children);

public sealed record MenuRowDto(
    ulong Id,
    ulong ApplicationId,
    string ApplicationName,
    ulong? ParentId,
    ulong? ResourceId,
    string? ResourceName,
    string? ResourceCode,
    string Code,
    string Name,
    string? Route,
    string? Icon,
    int SortOrder,
    bool IsVisible,
    bool IsActive,
    ulong Version);
