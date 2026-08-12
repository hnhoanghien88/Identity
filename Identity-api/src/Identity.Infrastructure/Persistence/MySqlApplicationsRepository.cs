using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using ApplicationEntity = Identity.Domain.Entities.Applications;

namespace Identity.Infrastructure.Persistence;

public sealed class MySqlApplicationsRepository(IdentityDbContext db) : IApplicationsRepository
{
    public Task<ApplicationEntity?> GetByIdAsync(ulong id, CancellationToken cancellationToken) =>
        db.Applications.SingleOrDefaultAsync(
            application => application.Id == id && !application.IsDeleted,
            cancellationToken);

    public Task<bool> CodeExistsAsync(string code, ulong? excludingId, CancellationToken cancellationToken) =>
        db.Applications.AnyAsync(
            application => application.Id != excludingId && application.Code == code,
            cancellationToken);

    public async Task<bool> HasDependenciesAsync(ulong id, CancellationToken cancellationToken) =>
        await db.Roles.AnyAsync(value => value.ApplicationId == id, cancellationToken)
        || await db.Resources.AnyAsync(value => value.ApplicationId == id, cancellationToken)
        || await db.Menus.AnyAsync(value => value.ApplicationId == id, cancellationToken)
        || await db.RefreshTokens.AnyAsync(value => value.ApplicationId == id, cancellationToken);

    public async Task AddAsync(ApplicationEntity application, CancellationToken cancellationToken)
    {
        db.Applications.Add(application);
        await SaveChangesAsync(cancellationToken);
    }

    public Task SaveAsync(ApplicationEntity application, CancellationToken cancellationToken) =>
        SaveChangesAsync(cancellationToken);

    private async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException("The Application was changed by another user. Reload and try again.");
        }
        catch (DbUpdateException exception)
        {
            var detail = exception.InnerException?.Message ?? exception.Message;
            if (detail.Contains("UQApplicationsCode", StringComparison.OrdinalIgnoreCase))
                throw new ConflictException("Code is already in use.", "code");
            throw new ConflictException("Application data conflicts with an existing record.");
        }
    }
}
