namespace HMSMini.API.Models.DTOs.MealPlan;

/// <summary>
/// DTO for creating a new meal plan rate
/// </summary>
public class CreateMealPlanRateDto
{
    public int MealPlanId { get; set; }
    public int RoomTypeId { get; set; }
    public decimal RatePerPersonPerNight { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}
