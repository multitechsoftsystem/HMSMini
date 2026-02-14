namespace HMSMini.API.Models.DTOs.MealPlan;

/// <summary>
/// DTO for meal plan rate display
/// </summary>
public class MealPlanRateDto
{
    public int Id { get; set; }
    public int MealPlanId { get; set; }
    public string MealPlanName { get; set; } = string.Empty;
    public string MealPlanCode { get; set; } = string.Empty;
    public int RoomTypeId { get; set; }
    public string RoomTypeName { get; set; } = string.Empty;
    public decimal RatePerPersonPerNight { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
