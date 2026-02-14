using System.ComponentModel.DataAnnotations;

namespace HMSMini.API.Models.DTOs.BanquetCharge;

public class UpdateBanquetChargeDto
{
    [Required]
    public DateTime ChargeDate { get; set; }

    [Required]
    [StringLength(50)]
    public string ChargeType { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    [Range(1, 100000)]
    public int Quantity { get; set; }

    public bool ApplyTax { get; set; } = true;

    public int? VoucherTaxConfigId { get; set; }
}
