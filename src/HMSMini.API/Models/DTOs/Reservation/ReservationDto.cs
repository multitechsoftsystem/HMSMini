using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.DTOs.Reservation;

/// <summary>
/// Data transfer object for reservation information
/// </summary>
public class ReservationDto
{
    public int Id { get; set; }
    public string ReservationNumber { get; set; } = string.Empty;
    public string? RoomNumber { get; set; }
    public string RoomTypeName { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfGuests { get; set; }
    public string GuestName { get; set; } = string.Empty;
    public string? GuestEmail { get; set; }
    public string GuestMobile { get; set; } = string.Empty;
    public string? SpecialRequests { get; set; }
    public ReservationStatus Status { get; set; }
    public int? CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public string? CompanyGSTNumber { get; set; }
    public int? BusinessSourceId { get; set; }
    public string? BusinessSourceName { get; set; }
    public int? MealPlanId { get; set; }
    public string? MealPlanName { get; set; }
    public decimal? EstimatedAmount { get; set; }
    public int? CheckInId { get; set; }
    public DateTime CreatedAt { get; set; }
}
