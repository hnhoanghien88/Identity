using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using ResourceEntity = Identity.Domain.Entities.Resources;

namespace Identity.Infrastructure.Persistence;

public sealed class MySqlResourcesRepository(IdentityDbContext db)
    : IResourcesRepository
{
    public Task<ResourceEntity?> GetByIdAsync(
        ulong id,
        CancellationToken cancellationToken) =>
        db.Resources
            .Include(resource => resource.Application)
            .SingleOrDefaultAsync(
                resource => resource.Id == id && !resource.IsDeleted,
                cancellationToken);

    public Task<bool> IsApplicationAvailableAsync(
        ulong id,
        CancellationToken cancellationToken) =>
        db.Applications.AnyAsync(
            application =>
                application.Id == id
                && application.IsActive
                && !application.IsDeleted,
            cancellationToken);

    public Task<bool> CodeExistsAsync(
        ulong applicationId,
        string code,
        ulong? excludingId,
        CancellationToken cancellationToken) =>
        db.Resources.AnyAsync(
            resource =>
                resource.Id != excludingId
                && resource.ApplicationId == applicationId
                && resource.Code == code,
            cancellationToken);

    public async Task<bool> HasDependenciesAsync(
        ulong id,
        CancellationToken cancellationToken) =>
        await db.Permissions.AnyAsync(
            value => value.ResourceId == id,
            cancellationToken)
        || await db.Menus.AnyAsync(
            value => value.ResourceId == id,
            cancellationToken);

    public async Task AddAsync(
        ResourceEntity resource,
        CancellationToken cancellationToken)
    {
        resource.Application = await AvailableApplicationAsync(
            resource.ApplicationId,
            cancellationToken);
        db.Resources.Add(resource);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task SaveAsync(
        ResourceEntity resource,
        CancellationToken cancellationToken)
    {
        resource.Application = await db.Applications.SingleOrDefaultAsync(
            application =>
                application.Id == resource.ApplicationId
                && !application.IsDeleted,
            cancellationToken)
            ?? throw new ConflictException(
                "The selected Application is unavailable.",
                "applicationId");
        await SaveChangesAsync(cancellationToken);
    }

    private async Task<Identity.Domain.Entities.Applications> AvailableApplicationAsync(
        ulong id,
        CancellationToken cancellationToken) =>
        await db.Applications.SingleOrDefaultAsync(
            application =>
                application.Id == id
                && application.IsActive
                && !application.IsDeleted,
            cancellationToken)
        ?? throw new ConflictException(
            "The selected Application is unavailable.",
            "applicationId");

    private async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException(
                "The Resource was changed by another user. Reload and try again.");
        }
        catch (DbUpdateException exception)
        {
            var detail = exception.InnerException?.Message ?? exception.Message;
            if (detail.Contains("UQResources", StringComparison.OrdinalIgnoreCase)
                || detail.Contains(
                    "resources_applicationid_code_unique",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new ConflictException(
                    "Code is already in use for this Application.",
                    "code");
            }
            throw new ConflictException(
                "Resource data conflicts with an existing record.");
        }
    }
}
