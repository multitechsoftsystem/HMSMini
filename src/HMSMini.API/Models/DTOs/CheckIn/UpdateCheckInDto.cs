namespace HMSMini.API.Models.DTOs.CheckIn;

public class UpdateCheckInDto
{
    public int? CompanyId { get; set; }
    public int? BusinessSourceId { get; set; }
    public int? MealPlanId { get; set; }
    public int? GuestTypeId { get; set; }
    public string? Remarks { get; set; }

    /// <summary>
    /// New checkout date (optional). Cannot be before working date.
    /// Must be after check-in date.
    /// </summary>
    public DateTime? CheckOutDate { get; set; }
}
