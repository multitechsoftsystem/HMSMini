using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSMini.API.Models.Entities;

[Table("BanquetBookingMenus")]
public class BanquetBookingMenu
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BanquetBookingId { get; set; }

    public int? MenuPackageId { get; set; }

    public int? MenuItemId { get; set; }

    [Required]
    public int Quantity { get; set; } = 1;

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal RatePerPlate { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalAmount { get; set; }

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
    [ForeignKey(nameof(BanquetBookingId))]
    public virtual BanquetBooking BanquetBooking { get; set; } = null!;

    [ForeignKey(nameof(MenuPackageId))]
    public virtual MMenuPackage? MenuPackage { get; set; }

    [ForeignKey(nameof(MenuItemId))]
    public virtual MMenuItem? MenuItem { get; set; }
}
