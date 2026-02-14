namespace HMSMini.API.Models.DTOs.MealPlan;

/// <summary>
/// DTO for meal plan display
/// </summary>
public class MealPlanDto
{
    public int MealPlanId { get; set; }
    public string PlanCode { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
