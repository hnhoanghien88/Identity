using System.Data;
using System.Security.Cryptography;
using System.Text;
using Identity.Application.Abstractions.Persistence;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence;

public sealed class MySqlRefreshTokenRepository(IdentityDbContext dbContext)
    : IRefreshTokenRepository
{
    public async Task<IssuedRefreshToken> IssueForLoginAsync(
        ulong userId,
        string userEmail,
        string applicationCode,
        TimeSpan lifetime,
        CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.Serializable, cancellationToken);

        var applicationId = await GetApplicationIdAsync(userId, applicationCode, cancellationToken);
        var now = DateTime.UtcNow;

        var activeTokens = await dbContext.RefreshTokens
            .Where(x => x.UserId == userId
                && x.ApplicationId == applicationId
                && x.IsActive
                && x.RevokedDate == null)
            .ToListAsync(cancellationToken);

        foreach (var activeToken in activeTokens)
        {
            activeToken.IsActive = false;
            activeToken.RevokedDate = now;
            activeToken.RevokedBy = userId;
            activeToken.UpdatedDate = now;
            activeToken.UpdatedBy = userEmail;
        }

        var issued = CreateToken(userId, applicationId, Guid.NewGuid(), lifetime, now, userEmail);
        dbContext.RefreshTokens.Add(issued.Entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new IssuedRefreshToken(issued.RawToken, issued.Entity.ExpiresDate);
    }

    public async Task<RotatedRefreshToken> RotateAsync(
        string refreshToken,
        TimeSpan lifetime,
        CancellationToken cancellationToken)
    {
        var tokenHash = HashToken(refreshToken);
        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.Serializable, cancellationToken);

        var current = await dbContext.RefreshTokens
            .Include(x => x.User)
            .SingleOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken)
            ?? throw new UnauthorizedAccessException("The refresh token is invalid.");

        var now = DateTime.UtcNow;
        if (current.User.IsDeleted || !current.User.IsActive)
            throw new UnauthorizedAccessException("The user account is unavailable.");

        if (!current.IsActive || current.RevokedDate != null)
        {
            await RevokeFamilyAsync(current.FamilyId, current.UserId, current.User.Email, now, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            throw new UnauthorizedAccessException(
                "Refresh token reuse was detected. The token family has been revoked.");
        }

        if (current.ExpiresDate <= now)
        {
            current.IsActive = false;
            current.RevokedDate = now;
            current.RevokedBy = current.UserId;
            current.UpdatedDate = now;
            current.UpdatedBy = current.User.Email;
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            throw new UnauthorizedAccessException("The refresh token has expired.");
        }

        var replacement = CreateToken(
            current.UserId, current.ApplicationId, current.FamilyId, lifetime, now, current.User.Email);
        dbContext.RefreshTokens.Add(replacement.Entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        current.IsActive = false;
        current.RevokedDate = now;
        current.RevokedBy = current.UserId;
        current.ReplacedByTokenId = replacement.Entity.Id;
        current.UpdatedDate = now;
        current.UpdatedBy = current.User.Email;

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new RotatedRefreshToken(
            current.UserId,
            replacement.RawToken,
            replacement.Entity.ExpiresDate);
    }

    public async Task RevokeAsync(
        string refreshToken,
        string? revokedBy,
        CancellationToken cancellationToken)
    {
        var tokenHash = HashToken(refreshToken);
        var token = await dbContext.RefreshTokens
            .Include(x => x.User)
            .SingleOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

        if (token is null || !token.IsActive)
            return;

        var now = DateTime.UtcNow;
        token.IsActive = false;
        token.RevokedDate = now;
        token.RevokedBy = token.UserId;
        token.UpdatedDate = now;
        token.UpdatedBy = revokedBy ?? token.User.Email;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeAllForUserAsync(
        ulong userId,
        string? revokedBy,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var tokens = await dbContext.RefreshTokens
            .Where(x => x.UserId == userId && x.IsActive)
            .ToListAsync(cancellationToken);
        foreach (var token in tokens)
        {
            token.IsActive = false;
            token.RevokedDate = now;
            token.RevokedBy = userId;
            token.UpdatedDate = now;
            token.UpdatedBy = revokedBy;
        }
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<ulong> GetApplicationIdAsync(
        ulong userId,
        string applicationCode,
        CancellationToken cancellationToken)
    {
        var applicationId = await dbContext.UserRoles
            .Where(x => x.UserId == userId
                && x.IsActive
                && x.Role.IsActive
                && !x.Role.IsDeleted
                && x.Role.Application.IsActive
                && !x.Role.Application.IsDeleted
                && x.Role.Application.Code == applicationCode)
            .OrderBy(x => x.Role.ApplicationId)
            .Select(x => x.Role.ApplicationId)
            .FirstOrDefaultAsync(cancellationToken);

        return applicationId != 0
            ? applicationId
            : throw new InvalidOperationException(
                "The user has no active application role.");
    }

    private async Task RevokeFamilyAsync(
        Guid familyId,
        ulong revokedBy,
        string revokedByEmail,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var family = await dbContext.RefreshTokens
            .Where(x => x.FamilyId == familyId && x.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var token in family)
        {
            token.IsActive = false;
            token.RevokedDate = now;
            token.RevokedBy = token.UserId;
            token.UpdatedDate = now;
            token.UpdatedBy = revokedByEmail;
        }
    }

    private static (RefreshTokens Entity, string RawToken) CreateToken(
        ulong userId,
        ulong applicationId,
        Guid familyId,
        TimeSpan lifetime,
        DateTime now,
        string userEmail)
    {
        var rawToken = GenerateToken();
        var entity = new RefreshTokens
        {
            UserId = userId,
            ApplicationId = applicationId,
            TokenHash = HashToken(rawToken),
            JwtId = Guid.NewGuid(),
            FamilyId = familyId,
            ExpiresDate = now.Add(lifetime),
            IsActive = true,
            CreatedDate = now,
            CreatedBy = userEmail
        };

        return (entity, rawToken);
    }

    private static string GenerateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static string HashToken(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)))
            .ToLowerInvariant();
}
