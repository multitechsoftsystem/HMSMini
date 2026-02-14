using System.ComponentModel.DataAnnotations;

namespace HMSMini.API.Models.DTOs.BanquetBookingMenu;

public class UpdateBanquetBookingMenuDto
{
    [Required]
    [Range(1, 100000)]
    public int Quantity { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal RatePerPlate { get; set; }
}
