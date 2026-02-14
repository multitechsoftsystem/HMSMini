namespace HMSMini.API.Models.DTOs.Tariff;

/// <summary>
/// DTO for base tariff read operations
/// </summary>
public class BaseTariffDto
{
    public int Id { get; set; }
    public int RoomTypeId { get; set; }
    public string RoomTypeName { get; set; } = string.Empty;
    public int OccupancyCount { get; set; }
    public decimal RatePerNight { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; }
}
