namespace Identity.Domain.Entities;

public sealed class Applications : ActiveEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public sealed class Roles : ActiveEntity
{
    public ulong ApplicationId { get; set; }
    public Applications Application { get; set; } = null!;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsSystemRole { get; set; }
}

public sealed class Resources : ActiveEntity
{
    public ulong ApplicationId { get; set; }
    public Applications Application { get; set; } = null!;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public string? Description { get; set; }
}