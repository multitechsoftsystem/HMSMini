using System.ComponentModel.DataAnnotations;

namespace HMSMini.API.Models.DTOs.Billing;

/// <summary>
/// DTO for creating a new additional charge
/// </summary>
public class CreateAdditionalChargeDto
{
    [Required]
    public DateTime ChargeDate { get; set; } = DateTime.Today;

    [Required]
    [StringLength(50)]
    public string ChargeType { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0.01, 999999.99)]
    public decimal Amount { get; set; }

    [Required]
    [Range(1, 1000)]
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// Whether tax should be applied to this charge
    /// </summary>
    public bool ApplyTax { get; set; } = true;

    /// <summary>
    /// Optional voucher-specific tax configuration ID
    /// </summary>
    public int? VoucherTaxConfigId { get; set; }
}
