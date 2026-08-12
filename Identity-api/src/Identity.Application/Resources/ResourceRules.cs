using Identity.Application.Resources.Dtos;
using ResourceEntity = Identity.Domain.Entities.Resources;

namespace Identity.Application.Resources;

public static class ResourceRules
{
    public static string Clean(string value) => value.Trim();
    public static string? CleanDescription(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    public static ResourceDto ToDto(ResourceEntity value) =>
        new(
            value.Id,
            value.ApplicationId,
            value.Application.Code,
            value.Application.Name,
            value.Code,
            value.Name,
            value.ResourceType,
            value.Description,
            value.CreatedDate,
            value.IsActive,
            value.Version);
}