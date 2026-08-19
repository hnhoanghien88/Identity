using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Exceptions;
using Identity.Application.Roles.Dtos;
using Microsoft.EntityFrameworkCore;
using RoleEntity = Identity.Domain.Entities.Roles;

namespace Identity.Infrastructure.Persistence;

public sealed class MySqlRolesRepository(IdentityDbContext db, IRolesReadRepository readRepository) : IRolesRepository
{
    public Task<RoleEntity?> GetByIdAsync(ulong id, CancellationToken cancellationToken) =>
        db.Roles.SingleOrDefaultAsync(role => role.Id == id && !role.IsDeleted, cancellationToken);

    public async Task<RoleDto> GetDtoAsync(ulong id, CancellationToken cancellationToken) =>
        await readRepository.GetByIdAsync(id, cancellationToken)
        ?? throw new InvalidOperationException("The saved Role could not be reloaded.");

    public Task<bool> ApplicationExistsAsync(ulong id, CancellationToken cancellationToken) =>
        db.Applications.AnyAsync(application => application.Id == id && !application.IsDeleted, cancellationToken);

    public Task<bool> CodeExistsAsync(
        ulong applicationId,
        string code,
        ulong? excludingId,
        CancellationToken cancellationToken) =>
        db.Roles.AnyAsync(
            role => role.Id != excludingId
                && role.ApplicationId == applicationId
                && role.Code == code,
            cancellationToken);

    public async Task<bool> HasDependenciesAsync(ulong id, CancellationToken cancellationToken) =>
        await db.UserRoles.AnyAsync(value => value.RoleId == id, cancellationToken)
        || await db.RolePermissions.AnyAsync(value => value.RoleId == id, cancellationToken);

    public async Task AddAsync(RoleEntity role, CancellationToken cancellationToken)
    {
        db.Roles.Add(role);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task SaveAsync(RoleEntity role, CancellationToken cancellationToken)
    {
        if (role.IsDeleted)
            await SoftDeleteCascade.SaveAsync(db, () => SoftDeleteCascade.RoleAsync(
                db, role.Id, role.UpdatedBy, role.UpdatedDate ?? DateTime.UtcNow, cancellationToken), cancellationToken);
        else
            await SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(RoleEntity role, CancellationToken cancellationToken)
    {
        db.Roles.Remove(role);
        await SaveChangesAsync(cancellationToken);
    }

    private async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException("The Role was changed by another user. Reload and try again.");
        }
        catch (DbUpdateException exception)
        {
            var detail = exception.InnerException?.Message ?? exception.Message;
            if (detail.Contains("UQRoles", StringComparison.OrdinalIgnoreCase))
                throw new ConflictException("Code is already in use for this Application.", "code");
            if (detail.Contains("RoleId", StringComparison.OrdinalIgnoreCase))
                throw new ConflictException("This Role cannot be deleted while related Users or Permissions exist.");
            if (detail.Contains("ApplicationId", StringComparison.OrdinalIgnoreCase))
                throw new ConflictException("The selected Application is unavailable.", "applicationId");
            throw new ConflictException("Role data conflicts with an existing record.");
        }
    }
}
