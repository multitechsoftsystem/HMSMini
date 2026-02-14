using System.ComponentModel.DataAnnotations;

namespace HMSMini.API.Models.DTOs.BanquetHall;

public class CreateBanquetHallDto
{
    [Required(ErrorMessage = "Hall name is required")]
    [StringLength(200)]
    public string HallName { get; set; } = string.Empty;

    [Required]
    [Range(1, 10000)]
    public int MaxCapacity { get; set; }

    [Range(0, 10000)]
    public int MinCapacity { get; set; }

    [Range(0, double.MaxValue)]
    public decimal RentPerEvent { get; set; }

    [StringLength(200)]
    public string? Location { get; set; }

    [StringLength(1000)]
    public string? Features { get; set; }

    [StringLength(500)]
    public string? ImagePath { get; set; }
}
