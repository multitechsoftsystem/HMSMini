using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSMini.API.Models.Entities;

[Table("MMenuPackages")]
public class MMenuPackage
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string PackageName { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")]
    public decimal RatePerPlate { get; set; }

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
    public virtual ICollection<MMenuPackageItem> PackageItems { get; set; } = new List<MMenuPackageItem>();
}
