using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Exceptions;
using Identity.Application.UserRoles;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace Identity.Infrastructure.Persistence;

public sealed class MySqlUserRolesRepository(IdentityDbContext db) : IUserRolesRepository
{
    public async Task<PagedUserRoleUsersDto> GetMembersAsync(ulong roleId, int page, int pageSize, CancellationToken ct)
    {
        await EnsureRoleAsync(roleId, ct);
        var q = db.UserRoles.AsNoTracking().Where(x => x.RoleId == roleId && x.IsActive && x.User.IsActive && !x.User.IsDeleted).OrderBy(x => x.User.Code).ThenBy(x => x.UserId).Select(x => new UserRoleUserDto(x.UserId, x.User.Code, x.User.DisplayName, x.User.Email));
        return await PageAsync(q, page, pageSize, ct);
    }
    public async Task<PagedUserRoleUsersDto> GetCandidatesAsync(ulong roleId, string? search, int page, int pageSize, CancellationToken ct)
    {
        await EnsureRoleAsync(roleId, ct);
        var q = db.Users.AsNoTracking().Where(u => u.IsActive && !u.IsDeleted && !db.UserRoles.Any(x => x.RoleId == roleId && x.UserId == u.Id && x.IsActive));
        if (!string.IsNullOrWhiteSpace(search)) q = q.Where(u => u.Code.Contains(search) || u.DisplayName.Contains(search) || u.Email.Contains(search));
        return await PageAsync(q.OrderBy(u => u.Code).ThenBy(u => u.Id).Select(u => new UserRoleUserDto(u.Id, u.Code, u.DisplayName, u.Email)), page, pageSize, ct);
    }
    public async Task<AssignUsersToRoleResultDto> AssignAsync(ulong roleId, IReadOnlyCollection<ulong> userIds, string? actor, CancellationToken ct)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        await EnsureRoleAsync(roleId, ct);
        var requested = userIds.Distinct().ToArray();
        var valid = await db.Users.Where(u => requested.Contains(u.Id) && u.IsActive && !u.IsDeleted).Select(u => u.Id).ToListAsync(ct);
        if (valid.Count != requested.Length) throw new NotFoundException("One or more Users were not found.");
        var existing = await db.UserRoles.Where(x => x.RoleId == roleId && requested.Contains(x.UserId)).Select(x => x.UserId).ToListAsync(ct);
        var assigned = requested.Except(existing).ToArray();
        db.UserRoles.AddRange(assigned.Select(userId => new UserRoles { RoleId = roleId, UserId = userId, IsActive = true, CreatedBy = actor, CreatedDate = DateTime.UtcNow }));
        try { await db.SaveChangesAsync(ct); await transaction.CommitAsync(ct); }
        catch (DbUpdateException) { throw new ConflictException("User Role data changed concurrently. Reload and try again."); }
        return new(roleId, assigned, existing);
    }
    public async Task RemoveAsync(ulong roleId, ulong userId, CancellationToken ct)
    {
        await EnsureRoleAsync(roleId, ct);
        if (!await db.Users.AnyAsync(u => u.Id == userId, ct)) throw new NotFoundException($"User '{userId}' was not found.");
        var membership = await db.UserRoles.SingleOrDefaultAsync(x => x.RoleId == roleId && x.UserId == userId, ct);
        if (membership is null) return;
        db.UserRoles.Remove(membership);
        await db.SaveChangesAsync(ct);
    }
    private async Task EnsureRoleAsync(ulong id, CancellationToken ct) { if (!await db.Roles.AnyAsync(r => r.Id == id && r.IsActive && !r.IsDeleted, ct)) throw new NotFoundException($"Role '{id}' was not found."); }
    private static async Task<PagedUserRoleUsersDto> PageAsync(IQueryable<UserRoleUserDto> q, int page, int pageSize, CancellationToken ct) { var total = await q.CountAsync(ct); var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct); return new(items, page, pageSize, total); }
}
