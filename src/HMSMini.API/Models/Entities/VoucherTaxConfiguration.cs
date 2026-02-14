using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSMini.API.Models.Entities;

/// <summary>
/// Tax configuration for different voucher/charge types
/// </summary>
[Table("VoucherTaxConfigurations")]
public class VoucherTaxConfiguration
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Type of voucher/charge (Room Service, Minibar, Laundry, Restaurant, etc.)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string VoucherType { get; set; } = string.Empty;

    /// <summary>
    /// CGST percentage for this voucher type
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal CgstPercentage { get; set; }

    /// <summary>
    /// SGST percentage for this voucher type
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal SgstPercentage { get; set; }

    /// <summary>
    /// IGST percentage for this voucher type
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal IgstPercentage { get; set; }

    /// <summary>
    /// Whether this configuration is currently active
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Description of this tax configuration
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Display order for UI
    /// </summary>
    public int DisplayOrder { get; set; }

    // Audit fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    [StringLength(100)]
    public string? DeletedBy { get; set; }

    // Navigation properties
    public virtual ICollection<AdditionalCharge> AdditionalCharges { get; set; } = new List<AdditionalCharge>();
}
