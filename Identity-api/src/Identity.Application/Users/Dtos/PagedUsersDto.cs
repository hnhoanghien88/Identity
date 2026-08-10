namespace Identity.Application.Users.Dtos;

public sealed record PagedUsersDto(
    IReadOnlyList<UsersDto> Items,
    int TotalCount,
    int Page,
    int PageSize);
