using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSMini.API.Models.Entities;

[Table("MMenuPackageItems")]
public class MMenuPackageItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int MenuPackageId { get; set; }

    [Required]
    public int MenuItemId { get; set; }

    // Audit fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    [StringLength(100)]
    public string? DeletedBy { get; set; }

    // Navigation properties
    [ForeignKey(nameof(MenuPackageId))]
    public virtual MMenuPackage MenuPackage { get; set; } = null!;

    [ForeignKey(nameof(MenuItemId))]
    public virtual MMenuItem MenuItem { get; set; } = null!;
}
