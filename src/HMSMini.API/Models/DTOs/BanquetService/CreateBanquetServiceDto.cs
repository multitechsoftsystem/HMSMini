using System.ComponentModel.DataAnnotations;

namespace HMSMini.API.Models.DTOs.BanquetService;

public class CreateBanquetServiceDto
{
    [Required(ErrorMessage = "Service name is required")]
    [StringLength(200)]
    public string ServiceName { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal DefaultRate { get; set; }

    [StringLength(50)]
    public string? Unit { get; set; }

    public bool ApplyTax { get; set; } = true;

    public int? VoucherTaxConfigId { get; set; }
}
