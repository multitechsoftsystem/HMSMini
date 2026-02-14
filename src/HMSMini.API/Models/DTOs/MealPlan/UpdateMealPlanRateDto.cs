namespace HMSMini.API.Models.DTOs.MealPlan;

/// <summary>
/// DTO for updating a meal plan rate
/// </summary>
public class UpdateMealPlanRateDto
{
    public int MealPlanId { get; set; }
    public int RoomTypeId { get; set; }
    public decimal RatePerPersonPerNight { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; }
}
