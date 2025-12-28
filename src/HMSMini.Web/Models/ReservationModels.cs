namespace HMSMini.Web.Models;

public class ReservationDto
{
    public int Id { get; set; }
    public string ReservationNumber { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public string RoomTypeName { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfGuests { get; set; }
    public string GuestName { get; set; } = string.Empty;
    public string? GuestEmail { get; set; }
    public string GuestMobile { get; set; } = string.Empty;
    public string? SpecialRequests { get; set; }
    public int Status { get; set; }
    public string StatusName => ((ReservationStatus)Status).ToString();
    public int? CheckInId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateReservationDto
{
    public string RoomNumber { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; } = DateTime.Today.AddDays(1);
    public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(3);
    public int NumberOfGuests { get; set; } = 1;
    public string GuestName { get; set; } = string.Empty;
    public string? GuestEmail { get; set; }
    public string GuestMobile { get; set; } = string.Empty;
    public string? SpecialRequests { get; set; }
}

public enum ReservationStatus
{
    Pending = 0,
    Confirmed = 1,
    CheckedIn = 2,
    Cancelled = 3,
    NoShow = 4
}
