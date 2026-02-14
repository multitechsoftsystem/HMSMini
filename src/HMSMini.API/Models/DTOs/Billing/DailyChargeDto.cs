namespace HMSMini.API.Models.DTOs.Billing;

/// <summary>
/// Represents charges for a single day
/// </summary>
public class DailyChargeDto
{
    public DateTime Date { get; set; }
    public decimal RoomRate { get; set; }
    public decimal MealPlanRate { get; set; }
    public decimal SubtotalBeforeDiscount { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal SubtotalAfterDiscount { get; set; }
    public List<TaxLineDto> Taxes { get; set; } = new();
    public decimal TotalTax { get; set; }
    public decimal DayTotal { get; set; }
    public string? RateChangeNote { get; set; } // e.g., "Rate change effective"
}
