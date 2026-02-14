using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.DTOs.BanquetBooking;

public class BanquetBookingListDto
{
    public int Id { get; set; }
    public string BookingNumber { get; set; } = string.Empty;
    public string HallName { get; set; } = string.Empty;
    public string EventTypeName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public TimeSpan EventStartTime { get; set; }
    public TimeSpan EventEndTime { get; set; }
    public int ExpectedGuests { get; set; }
    public BanquetBookingStatus Status { get; set; }
    public string ContactPersonName { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public decimal HallRent { get; set; }
}
