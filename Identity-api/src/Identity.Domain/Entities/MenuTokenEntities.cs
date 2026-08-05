namespace Identity.Domain.Entities;

public sealed class Menus : ActiveEntity
{
    public ulong ApplicationId { get; set; }
    public Applications Application { get; set; } = null!;
    public ulong? ParentId { get; set; }
    public Menus? Parent { get; set; }
    public ulong? ResourceId { get; set; }
    public Resources? Resource { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Route { get; set; }
    public string? Icon { get; set; }
    public int SortOrder { get; set; }
    public bool IsVisible { get; set; } = true;
}

public sealed class RefreshTokens : EntityBase
{
    public ulong UserId { get; set; }
    public Users User { get; set; } = null!;
    public ulong ApplicationId { get; set; }
    public Applications Application { get; set; } = null!;
    public string TokenHash { get; set; } = string.Empty;
    public Guid JwtId { get; set; }
    public Guid FamilyId { get; set; }
    public DateTime ExpiresDate { get; set; }
    public DateTime? RevokedDate { get; set; }
    public ulong? RevokedBy { get; set; }
    public ulong? ReplacedByTokenId { get; set; }
    public RefreshTokens? ReplacedByToken { get; set; }
    public bool IsActive { get; set; } = true;
}
