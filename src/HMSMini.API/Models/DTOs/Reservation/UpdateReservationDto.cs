using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.DTOs.Reservation;

/// <summary>
/// Data transfer object for updating a reservation
/// </summary>
public class UpdateReservationDto
{
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public int? NumberOfGuests { get; set; }
    public string? GuestName { get; set; }
    public string? GuestEmail { get; set; }
    public string? GuestMobile { get; set; }
    public string? SpecialRequests { get; set; }
    public ReservationStatus? Status { get; set; }
}
