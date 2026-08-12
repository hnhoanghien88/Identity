using Identity.Application.Roles.Dtos;
using RoleEntity = Identity.Domain.Entities.Roles;

namespace Identity.Application.Roles;

public static class RoleRules
{
    public static string Clean(string value) => value.Trim();

    public static RoleDto ToDto(RoleEntity value) =>
        new(
            value.Id,
            value.ApplicationId,
            value.Application?.Code ?? string.Empty,
            value.Application?.Name ?? string.Empty,
            value.Code,
            value.Name,
            value.IsSystemRole,
            value.IsActive,
            value.CreatedDate,
            value.Version);
}
