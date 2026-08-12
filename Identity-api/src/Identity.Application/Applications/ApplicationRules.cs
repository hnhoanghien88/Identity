using Identity.Application.Applications.Dtos;
using ApplicationEntity = Identity.Domain.Entities.Applications;

namespace Identity.Application.Applications;

public static class ApplicationRules
{
    public static string Clean(string value) => value.Trim();
    public static string? CleanDescription(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    public static ApplicationDto ToDto(ApplicationEntity value) => new(value.Id, value.Code, value.Name, value.Audience, value.Description, value.CreatedDate, value.IsActive, value.Version);
}
