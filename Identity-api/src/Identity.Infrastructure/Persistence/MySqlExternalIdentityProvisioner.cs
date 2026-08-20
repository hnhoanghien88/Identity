using Identity.Application.Abstractions.Persistence;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence;

public sealed class MySqlExternalIdentityProvisioner(IdentityDbContext db) : IExternalIdentityProvisioner
{
    public async Task<Identity.Domain.Entities.Users> ProvisionAsync(
        ExternalIdentityProfile profile,
        string applicationCode,
        string defaultRoleCode,
        CancellationToken cancellationToken)
    {
        Validate(profile, applicationCode, defaultRoleCode);
        var now = DateTime.UtcNow;
        var identity = await db.ExternalIdentities.IgnoreQueryFilters()
            .Include(x => x.User)
            .SingleOrDefaultAsync(
                x => x.Provider == profile.Provider && x.ProviderSubject == profile.Subject,
                cancellationToken);

        if (identity is not null)
        {
            if (!identity.IsActive || identity.IsDeleted || !identity.User.IsActive || identity.User.IsDeleted)
                throw new UnauthorizedAccessException("The external account is unavailable.");

            identity.DisplayName = profile.DisplayName;
            identity.AvatarUrl = profile.AvatarUrl;
            identity.LastLoginDate = now;
            identity.UpdatedBy = "external:google";
            identity.UpdatedDate = now;
            await db.SaveChangesAsync(cancellationToken);
            return identity.User;
        }

        if (await db.Users.IgnoreQueryFilters().AnyAsync(x => x.Email == profile.Email, cancellationToken))
            throw new InvalidOperationException("The external account cannot be provisioned.");

        var email = profile.Email.Trim();
        if (await db.Users.IgnoreQueryFilters().AnyAsync(x => x.Code == email, cancellationToken))
            throw new InvalidOperationException("The external account cannot be provisioned.");

        var application = await db.Applications.SingleOrDefaultAsync(
            x => x.Code == applicationCode && x.IsActive,
            cancellationToken)
            ?? throw new InvalidOperationException("The external login application is unavailable.");
        var role = await db.Roles.SingleOrDefaultAsync(
            x => x.ApplicationId == application.Id && x.Code == defaultRoleCode && x.IsActive,
            cancellationToken)
            ?? throw new InvalidOperationException("The external login default role is unavailable.");

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var actor = "external:google";
        var user = new Users
        {
            Code = email,
            Email = email,
            DisplayName = profile.DisplayName.Trim(),
            PasswordHash = null,
            IsActive = true,
            CreatedBy = actor,
            CreatedDate = now
        };
        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        db.ExternalIdentities.Add(new ExternalIdentity
        {
            UserId = user.Id,
            Provider = profile.Provider,
            ProviderSubject = profile.Subject,
            EmailAtLinkTime = email,
            DisplayName = profile.DisplayName.Trim(),
            AvatarUrl = profile.AvatarUrl,
            LastLoginDate = now,
            IsActive = true,
            CreatedBy = actor,
            CreatedDate = now
        });
        db.UserRoles.Add(new UserRoles
        {
            UserId = user.Id,
            RoleId = role.Id,
            IsActive = true,
            CreatedBy = actor,
            CreatedDate = now
        });
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return user;
    }

    private static void Validate(
        ExternalIdentityProfile profile,
        string applicationCode,
        string defaultRoleCode)
    {
        if (profile.Provider != "Google" || string.IsNullOrWhiteSpace(profile.Subject))
            throw new UnauthorizedAccessException("The external identity is invalid.");
        if (!profile.EmailVerified || string.IsNullOrWhiteSpace(profile.Email))
            throw new UnauthorizedAccessException("A verified email address is required.");
        if (string.IsNullOrWhiteSpace(profile.DisplayName))
            throw new UnauthorizedAccessException("The external identity profile is incomplete.");
        if (string.IsNullOrWhiteSpace(applicationCode) || string.IsNullOrWhiteSpace(defaultRoleCode))
            throw new InvalidOperationException("External login provisioning is not configured.");
    }
}
