namespace Identity.Application.Users.Dtos;

public sealed record UsersDto(
    ulong Id,
    string Code,
    string Name,
    DateTime CreatedDate,
    bool IsActive
);
