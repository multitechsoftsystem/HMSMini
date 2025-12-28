namespace HMSMini.Web.Models;

public class CheckInDto
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int Pax { get; set; }
    public int Status { get; set; }
    public string StatusName => ((CheckInStatus)Status).ToString();
    public DateTime? ActualCheckInDate { get; set; }
    public DateTime? ActualCheckOutDate { get; set; }
}

public class CheckInWithGuestsDto : CheckInDto
{
    public List<GuestDto> Guests { get; set; } = new();
}

public class CreateCheckInDto
{
    public string RoomNumber { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; } = DateTime.Today;
    public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(2);
    public List<CreateGuestDto> Guests { get; set; } = new();
}

public enum CheckInStatus
{
    Active = 0,
    CheckedOut = 1,
    Cancelled = 2
}
