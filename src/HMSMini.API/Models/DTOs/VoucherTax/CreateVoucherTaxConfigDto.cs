using System.ComponentModel.DataAnnotations;

namespace HMSMini.API.Models.DTOs.VoucherTax;

public class CreateVoucherTaxConfigDto
{
    [Required]
    [StringLength(100)]
    public string VoucherType { get; set; } = string.Empty;

    [Required]
    [Range(0, 100)]
    public decimal CgstPercentage { get; set; }

    [Required]
    [Range(0, 100)]
    public decimal SgstPercentage { get; set; }

    [Required]
    [Range(0, 100)]
    public decimal IgstPercentage { get; set; }

    public bool IsActive { get; set; } = true;

    [StringLength(500)]
    public string? Description { get; set; }

    public int DisplayOrder { get; set; }
}
