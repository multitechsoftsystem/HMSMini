using System.ComponentModel.DataAnnotations;

namespace HMSMini.API.Models.DTOs.BanquetBookingMenu;

public class CreateBanquetBookingMenuDto
{
    public int? MenuPackageId { get; set; }

    public int? MenuItemId { get; set; }

    [Required]
    [Range(1, 100000)]
    public int Quantity { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal RatePerPlate { get; set; }
}
