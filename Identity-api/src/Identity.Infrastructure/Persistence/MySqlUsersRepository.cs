using Identity.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Identity.Application.Common.Exceptions;
using Identity.Application.Users;
using UsersEntity = Identity.Domain.Entities.Users;

namespace Identity.Infrastructure.Persistence;

public sealed class MySqlUsersRepository(IdentityDbContext db) : IUsersRepository
{
    public async Task AddAsync(UsersEntity user, CancellationToken ct)
    {
        db.Users.Add(user);
        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException exception)
        {
            throw MapUniqueConflict(exception);
        }
    }

    public Task<UsersEntity?> GetByIdAsync(ulong id, CancellationToken ct) =>
        db.Users.SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);


    public Task<UsersEntity?> GetByCodeAsync(string code, CancellationToken ct)
    {
        return db.Users.SingleOrDefaultAsync(
            x => x.Code == code
                && x.IsActive
                && !x.IsDeleted,
            ct);
    }

    public Task<bool> CodeExistsAsync(string code, ulong? excludingId, CancellationToken ct)
    {
        return db.Users.AnyAsync(
            x => x.Id != excludingId
                && x.Code == code,
            ct);
    }

    public Task<bool> EmailExistsAsync(string email, ulong? excludingId, CancellationToken ct)
    {
        var value = UserIdentityRules.CleanEmail(email);
        return db.Users.AnyAsync(
            x => x.Id != excludingId
                && x.Email == value,
            ct);
    }

    public async Task UpdateAsync(UsersEntity user, CancellationToken ct)
    {
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateConcurrencyException) { throw new ConflictException(); }
        catch (DbUpdateException exception)
        {
            throw MapUniqueConflict(exception);
        }
    }

    public async Task DeleteAsync(UsersEntity user, CancellationToken ct)
    {
        await UpdateAsync(user, ct);
    }

    private static ConflictException MapUniqueConflict(
        DbUpdateException exception)
    {
        var detail = exception.InnerException?.Message ?? exception.Message;

        if (detail.Contains(
            "UQUsersCode",
            StringComparison.OrdinalIgnoreCase))
        {
            return new ConflictException(
                "Code is already in use.",
                "code");
        }

        if (detail.Contains(
            "UQUsersEmail",
            StringComparison.OrdinalIgnoreCase))
        {
            return new ConflictException(
                "Email is already in use.",
                "email");
        }

        return new ConflictException("User data conflicts with an existing record.");
    }
}
