using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.Entities;

[Table("MMenuItems")]
public class MMenuItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int MenuCategoryId { get; set; }

    [Required]
    [StringLength(200)]
    public string ItemName { get; set; } = string.Empty;

    [Required]
    public MenuItemType ItemType { get; set; } = MenuItemType.Veg;

    [Column(TypeName = "decimal(10,2)")]
    public decimal PricePerPlate { get; set; }

    public bool IsActive { get; set; } = true;

    // Audit fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    [StringLength(100)]
    public string? DeletedBy { get; set; }

    // Navigation properties
    [ForeignKey(nameof(MenuCategoryId))]
    public virtual MMenuCategory MenuCategory { get; set; } = null!;

    public virtual ICollection<MMenuPackageItem> PackageItems { get; set; } = new List<MMenuPackageItem>();
}
