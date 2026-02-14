using System.ComponentModel.DataAnnotations;

namespace HMSMini.API.Models.DTOs.Tariff;

/// <summary>
/// DTO for creating a company tariff
/// </summary>
public class CreateCompanyTariffDto
{
    [Required]
    public int CompanyId { get; set; }

    [Required]
    public int RoomTypeId { get; set; }

    [Required]
    [Range(1, 6)]
    public int OccupancyCount { get; set; }

    [Required]
    [Range(0, 999999.99)]
    public decimal RatePerNight { get; set; }

    [Required]
    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }
}
