namespace Identity.Application.Abstractions.Persistence;

public interface IRefreshTokenRepository
{
    Task<IssuedRefreshToken> IssueForLoginAsync(
        ulong userId,
        string userEmail,
        string applicationCode,
        TimeSpan lifetime,
        CancellationToken cancellationToken);

    Task<RotatedRefreshToken> RotateAsync(
        string refreshToken,
        TimeSpan lifetime,
        CancellationToken cancellationToken);

    Task RevokeAsync(
        string refreshToken,
        string? revokedBy,
        CancellationToken cancellationToken);

    Task RevokeAllForUserAsync(
        ulong userId,
        string? revokedBy,
        CancellationToken cancellationToken);
}

public sealed record IssuedRefreshToken(
    string Token,
    DateTime ExpiresAtUtc);

public sealed record RotatedRefreshToken(
    ulong UserId,
    string Token,
    DateTime ExpiresAtUtc);
