namespace Identity.Application.Resources.Dtos;

public sealed record ResourceDto(
    ulong Id,
    ulong ApplicationId,
    string ApplicationCode,
    string ApplicationName,
    string Code,
    string Name,
    string ResourceType,
    string? Description,
    DateTime CreatedDate,
    bool IsActive,
    ulong Version);