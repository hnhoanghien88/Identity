namespace Identity.Application.Applications.Dtos;

public sealed record ApplicationDto(ulong Id, string Code, string Name, string Audience, string? Description, DateTime CreatedDate, bool IsActive, ulong Version);
