using Identity.Application.Users.Dtos;
using Identity.Application.Users.GetUsers;

namespace Identity.Application.Abstractions.Persistence;

public interface IUsersReadRepository
{
    Task<UsersDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<UsersDto>> GetAsync(
        UsersFilter filter,
        IReadOnlyList<UsersSort> sorts,
        int page,
        int pageSize,
        CancellationToken cancellationToken);
}
