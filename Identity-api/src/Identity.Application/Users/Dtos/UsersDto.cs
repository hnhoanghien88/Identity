namespace Identity.Application.Users.Dtos;

public sealed record UsersDto(
    Guid Id,
    string Code,
    string Name,
    DateTime CreatedDate,
    bool IsActive
);
