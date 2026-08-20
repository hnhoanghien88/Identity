namespace Identity.Domain.Entities;

public sealed class ExternalIdentity : ActiveEntity
{
    public ulong UserId { get; set; }
    public Users User { get; set; } = null!;
    public string Provider { get; set; } = string.Empty;
    public string ProviderSubject { get; set; } = string.Empty;
    public string EmailAtLinkTime { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public DateTime LastLoginDate { get; set; }
}
