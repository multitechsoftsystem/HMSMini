using System.ComponentModel.DataAnnotations;

namespace HMSMini.API.Models.DTOs.BanquetBookingService;

public class CreateBanquetBookingServiceDto
{
    public int? BanquetServiceId { get; set; }

    public DateTime ServiceDate { get; set; }

    [Required]
    [StringLength(200)]
    public string ServiceName { get; set; } = string.Empty;

    [Required]
    [Range(1, 100000)]
    public int Quantity { get; set; } = 1;

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Rate { get; set; }

    public bool ApplyTax { get; set; } = true;

    public int? VoucherTaxConfigId { get; set; }
}
