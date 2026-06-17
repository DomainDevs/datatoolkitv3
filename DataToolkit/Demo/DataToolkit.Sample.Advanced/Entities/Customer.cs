using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataToolkit.Sample.Basic.Entities;

[Table("Customers")]
public class Customer
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Email { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
