using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSMini.API.Models.Entities;

/// <summary>
/// Additional charges for check-ins (room service, minibar, laundry, etc.)
/// </summary>
[Table("AdditionalCharges")]
public class AdditionalCharge
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Foreign key to check-in
    /// </summary>
    [Required]
    public int CheckInId { get; set; }

    /// <summary>
    /// Date when charge was incurred
    /// </summary>
    [Required]
    public DateTime ChargeDate { get; set; }

    /// <summary>
    /// Type of charge (Room Service, Minibar, Laundry, Telephone, Other)
    /// </summary>
    [Required]
    [StringLength(50)]
    public string ChargeType { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the charge
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Amount per unit
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Quantity (default 1)
    /// </summary>
    [Required]
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// Total amount (computed: Amount * Quantity)
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Whether tax should be applied to this charge
    /// </summary>
    [Required]
    public bool ApplyTax { get; set; } = true;

    /// <summary>
    /// Foreign key to voucher tax configuration (if null, uses check-in tax type)
    /// </summary>
    public int? VoucherTaxConfigId { get; set; }

    /// <summary>
    /// User who added this charge
    /// </summary>
    [StringLength(100)]
    public string? AddedBy { get; set; }

    // Audit fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Soft delete
    public DateTime? DeletedAt { get; set; }

    [StringLength(100)]
    public string? DeletedBy { get; set; }

    // Voucher posting tracking
    /// <summary>
    /// Indicates if this additional charge has been posted to a voucher
    /// </summary>
    public bool IsPostedToVoucher { get; set; } = false;

    /// <summary>
    /// Foreign key to the voucher that was created for this additional charge
    /// </summary>
    public int? PostedVoucherId { get; set; }

    // Navigation properties
    [ForeignKey(nameof(CheckInId))]
    public virtual CheckIn CheckIn { get; set; } = null!;

    [ForeignKey(nameof(VoucherTaxConfigId))]
    public virtual VoucherTaxConfiguration? VoucherTaxConfig { get; set; }

    [ForeignKey(nameof(PostedVoucherId))]
    public virtual Voucher? PostedVoucher { get; set; }
}
