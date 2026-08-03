using UsersEntity = Identity.Domain.Entities.Users;

namespace Identity.Application.Abstractions.Persistence;

public interface IUsersRepository
{
    Task AddAsync(UsersEntity user, CancellationToken cancellationToken);
    Task<UsersEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<UsersEntity?> GetByCodeAsync(string code, CancellationToken cancellationToken);
    Task<bool> CodeExistsAsync(string code, Guid? excludingId, CancellationToken cancellationToken);
    Task UpdateAsync(UsersEntity user, CancellationToken cancellationToken);
    Task DeleteAsync(UsersEntity user, CancellationToken cancellationToken);
}
