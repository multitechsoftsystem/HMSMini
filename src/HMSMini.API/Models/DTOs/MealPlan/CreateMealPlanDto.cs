namespace HMSMini.API.Models.DTOs.MealPlan;

/// <summary>
/// DTO for creating a new meal plan
/// </summary>
public class CreateMealPlanDto
{
    public string PlanCode { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public string? Description { get; set; }
}
