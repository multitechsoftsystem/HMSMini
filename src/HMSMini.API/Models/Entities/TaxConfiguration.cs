using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSMini.API.Models.Entities;

/// <summary>
/// Configurable tax rules (GST, CGST, SGST, etc.)
/// </summary>
[Table("TaxConfiguration")]
public class TaxConfiguration
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Type of tax (GST, CGST, SGST, VAT, etc.)
    /// </summary>
    [Required]
    [StringLength(50)]
    public string TaxType { get; set; } = string.Empty;

    /// <summary>
    /// Tax percentage (e.g., 9.00 for 9%)
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal TaxPercentage { get; set; }

    /// <summary>
    /// What this tax applies to: RoomCharges, MealCharges, AdditionalCharges, or All
    /// </summary>
    [Required]
    [StringLength(50)]
    public string ApplicableOn { get; set; } = "All";

    /// <summary>
    /// Start date for this tax rate
    /// </summary>
    [Required]
    public DateTime EffectiveFrom { get; set; }

    /// <summary>
    /// End date for this tax rate (null means no end date)
    /// </summary>
    public DateTime? EffectiveTo { get; set; }

    /// <summary>
    /// Whether this tax configuration is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Audit fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    // Soft delete
    public DateTime? DeletedAt { get; set; }

    [StringLength(100)]
    public string? DeletedBy { get; set; }
}
