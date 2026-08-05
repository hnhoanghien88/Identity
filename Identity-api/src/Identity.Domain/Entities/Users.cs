namespace Identity.Domain.Entities;

public sealed class Users : ActiveEntity
{
    public string Email { get; set; } = string.Empty;
    public string NormalizedEmail { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public Guid SecurityStamp { get; set; } = Guid.NewGuid();
    public int PermissionVersion { get; set; } = 1;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public string Code { get => Email; set { Email = value; NormalizedEmail = value.Trim().ToUpperInvariant(); } }
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public string Name { get => DisplayName; set => DisplayName = value; }
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public string Password { get => PasswordHash ?? string.Empty; set => PasswordHash = value; }
}
