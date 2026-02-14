using System.ComponentModel.DataAnnotations;

namespace HMSMini.API.Models.DTOs.BanquetBookingService;

public class UpdateBanquetBookingServiceDto
{
    [Required]
    [StringLength(200)]
    public string ServiceName { get; set; } = string.Empty;

    [Required]
    [Range(1, 100000)]
    public int Quantity { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Rate { get; set; }

    public bool ApplyTax { get; set; } = true;

    public int? VoucherTaxConfigId { get; set; }
}
