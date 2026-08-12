namespace Identity.Application.Roles.Dtos;

public sealed record RoleDto(
    ulong Id,
    ulong ApplicationId,
    string ApplicationCode,
    string ApplicationName,
    string Code,
    string Name,
    bool IsSystemRole,
    bool IsActive,
    DateTime CreatedDate,
    ulong Version);

public sealed record PagedRolesDto(IReadOnlyList<RoleDto> Items, int TotalCount, int Page, int PageSize);
