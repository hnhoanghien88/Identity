namespace Identity.Application.Actions.Dtos;

public sealed record ActionDto(ulong Id, string Code, string Name, DateTime CreatedDate, ulong Version);

