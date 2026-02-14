namespace HMSMini.API.Models.DTOs.MealPlan;

/// <summary>
/// DTO for updating a meal plan
/// </summary>
public class UpdateMealPlanDto
{
    public string PlanCode { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
