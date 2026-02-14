using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSMini.API.Models.Entities;

[Table("BanquetCharges")]
public class BanquetCharge
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BanquetBookingId { get; set; }

    [Required]
    public DateTime ChargeDate { get; set; }

    [Required]
    [StringLength(50)]
    public string ChargeType { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Amount { get; set; }

    [Required]
    public int Quantity { get; set; } = 1;

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalAmount { get; set; }

    public bool ApplyTax { get; set; } = true;

    public int? VoucherTaxConfigId { get; set; }

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

    [ForeignKey(nameof(VoucherTaxConfigId))]
    public virtual VoucherTaxConfiguration? VoucherTaxConfig { get; set; }
}
