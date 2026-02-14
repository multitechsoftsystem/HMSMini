namespace HMSMini.Web.Models;

public class MealPlanModel
{
    public int MealPlanId { get; set; }
    public string PlanCode { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateMealPlanModel
{
    public string PlanCode { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateMealPlanModel
{
    public string? PlanCode { get; set; }
    public string? PlanName { get; set; }
    public string? Description { get; set; }
}

public class MealPlanRateModel
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

public class CreateMealPlanRateModel
{
    public int MealPlanId { get; set; }
    public int RoomTypeId { get; set; }
    public decimal RatePerPersonPerNight { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}

public class UpdateMealPlanRateModel
{
    public decimal? RatePerPersonPerNight { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}
