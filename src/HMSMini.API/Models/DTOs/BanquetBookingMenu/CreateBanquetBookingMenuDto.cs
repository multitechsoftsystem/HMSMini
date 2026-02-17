using System.ComponentModel.DataAnnotations;

namespace HMSMini.API.Models.DTOs.BanquetBookingMenu;

public class CreateBanquetBookingMenuDto
{
    public int? MenuPackageId { get; set; }

    public int? MenuItemId { get; set; }

    public string? ItemName { get; set; }

    public DateTime MenuDate { get; set; }

    [Required]
    [Range(1, 100000)]
    public int Quantity { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal RatePerPlate { get; set; }

    public bool ApplyTax { get; set; } = true;

    public int? VoucherTaxConfigId { get; set; }
}
