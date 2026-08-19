namespace Identity.Domain.Entities;

public sealed class RateLimitPolicy : ActiveEntity
{
    public ulong? ApplicationId { get; set; }
    public Applications? Application { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RoutePattern { get; set; } = "*";
    public string? HttpMethods { get; set; }
    public string PartitionBy { get; set; } = string.Empty;
    public string Algorithm { get; set; } = string.Empty;
    public uint PermitLimit { get; set; }
    public uint WindowSeconds { get; set; }
    public uint? BurstLimit { get; set; }
    public int Priority { get; set; }
    public ulong Version { get; set; } = 1;
}