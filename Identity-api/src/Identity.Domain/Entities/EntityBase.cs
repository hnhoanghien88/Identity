namespace Identity.Domain.Entities;

public abstract class EntityBase
{
    public ulong Id { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
}

public abstract class ActiveEntity : EntityBase
{
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
}
