using Identity.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using UsersEntity = Identity.Domain.Entities.Users;

namespace Identity.Infrastructure.Persistence;

public sealed class MySqlUsersRepository(IdentityDbContext db) : IUsersRepository
{
    public async Task AddAsync(UsersEntity user, CancellationToken ct)
    {
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
    }

    public Task<UsersEntity?> GetByIdAsync(ulong id, CancellationToken ct) =>
        db.Users.SingleOrDefaultAsync(x => x.Id == id, ct);


    public Task<UsersEntity?> GetByCodeAsync(string code, CancellationToken ct)
    {
        var value = code.Trim().ToUpperInvariant();
        return db.Users.SingleOrDefaultAsync(x => x.NormalizedEmail == value, ct);
    }
    public Task<bool> CodeExistsAsync(string code, ulong? excludingId, CancellationToken ct)
    {
        var value = code.Trim().ToUpperInvariant();
        return db.Users.AnyAsync(x => x.Id != excludingId && x.NormalizedEmail == value, ct);
    }

    public async Task UpdateAsync(UsersEntity user, CancellationToken ct) =>
        await db.SaveChangesAsync(ct);

    public async Task DeleteAsync(UsersEntity user, CancellationToken ct)
    {
        db.Users.Remove(user);
        await db.SaveChangesAsync(ct);
    }
}
