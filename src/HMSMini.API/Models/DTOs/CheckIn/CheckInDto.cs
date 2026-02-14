using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.DTOs.CheckIn;

/// <summary>
/// CheckIn data transfer object
/// </summary>
public class CheckInDto
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public DateTime? ActualCheckInDate { get; set; }
    public DateTime? ActualCheckOutDate { get; set; }
    public string? RegistrationNo { get; set; }
    public int Pax { get; set; }
    public CheckInStatus Status { get; set; }
    public string? Remarks { get; set; }
    public int? CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public string? CompanyGSTNumber { get; set; }
    public int? BusinessSourceId { get; set; }
    public string? BusinessSourceName { get; set; }
    public int? MealPlanId { get; set; }
    public string? MealPlanName { get; set; }
    public int? GuestTypeId { get; set; }
    public string? GuestTypeName { get; set; }
    public decimal? MealPlanRate { get; set; }
    public decimal? TariffApplied { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal? FinalAmount { get; set; }
    public TaxType TaxType { get; set; }
    public string GuestNames { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}
