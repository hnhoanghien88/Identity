using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Identity.Domain.Entities;

public sealed class Users
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [Column(TypeName = "varchar(50)")]
    public string Code { get; set; } = string.Empty;

    [Column(TypeName = "varchar(255)")]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [Column(TypeName = "varchar(255)")]
    public string Password { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
}
