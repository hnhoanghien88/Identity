namespace Identity.Domain.Entities;

public sealed class PermissionActions : EntityBase
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public sealed class Permissions : ActiveEntity
{
    public ulong ApplicationId { get; set; }
    public Applications Application { get; set; } = null!;
    public ulong ResourceId { get; set; }
    public Resources Resource { get; set; } = null!;
    public ulong ActionId { get; set; }
    public PermissionActions Action { get; set; } = null!;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public sealed class UserRoles : EntityBase
{
    public ulong UserId { get; set; }
    public Users User { get; set; } = null!;
    public ulong RoleId { get; set; }
    public Roles Role { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}

public sealed class RolePermissions : EntityBase
{
    public ulong RoleId { get; set; }
    public Roles Role { get; set; } = null!;
    public ulong PermissionId { get; set; }
    public Permissions Permission { get; set; } = null!;
}