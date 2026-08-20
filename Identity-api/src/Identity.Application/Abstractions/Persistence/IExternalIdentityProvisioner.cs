namespace Identity.Application.Abstractions.Persistence;

public sealed record ExternalIdentityProfile(
    string Provider,
    string Subject,
    string Email,
    bool EmailVerified,
    string DisplayName,
    string? AvatarUrl);

public interface IExternalIdentityProvisioner
{
    Task<Identity.Domain.Entities.Users> ProvisionAsync(
        ExternalIdentityProfile profile,
        string applicationCode,
        string defaultRoleCode,
        CancellationToken cancellationToken);
}
