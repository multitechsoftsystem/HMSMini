namespace HMSMini.API.Models.DTOs.Reservation;

/// <summary>
/// Data transfer object for creating a new reservation
/// </summary>
public class CreateReservationDto
{
    public string RoomNumber { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfGuests { get; set; }
    public string GuestName { get; set; } = string.Empty;
    public string? GuestEmail { get; set; }
    public string GuestMobile { get; set; } = string.Empty;
    public string? SpecialRequests { get; set; }
}
