namespace HMSMini.API.Models.DTOs.Tariff;

/// <summary>
/// DTO for company tariff read operations
/// </summary>
public class CompanyTariffDto
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public int RoomTypeId { get; set; }
    public string RoomTypeName { get; set; } = string.Empty;
    public int OccupancyCount { get; set; }
    public decimal RatePerNight { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; }
}
