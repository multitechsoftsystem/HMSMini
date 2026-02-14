namespace HMSMini.API.Models.DTOs.Tariff;

/// <summary>
/// DTO for tariff calculation with detailed breakdown
/// </summary>
public class TariffCalculationDto
{
    public int RoomTypeId { get; set; }
    public string RoomTypeName { get; set; } = string.Empty;
    public int OccupancyCount { get; set; }
    public decimal BaseRate { get; set; }
    public decimal? CompanyRate { get; set; }
    public decimal ApplicableRate { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal FinalRate { get; set; }
    public int NumberOfNights { get; set; }
    public decimal TotalAmount { get; set; }
    public string TariffSource { get; set; } = string.Empty; // "Base", "Company", "CompanyWithDiscount"

    // Meal Plan fields
    public int? MealPlanId { get; set; }
    public string? MealPlanName { get; set; }
    public decimal MealPlanRatePerPerson { get; set; }
    public decimal MealPlanTotalRate { get; set; }
    public decimal TotalMealPlanAmount { get; set; }
    public decimal BaseRateWithMealPlan { get; set; }
    public decimal TotalAmountWithMealPlan { get; set; }
}
