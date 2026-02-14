namespace HMSMini.API.Models.DTOs.Guest;

/// <summary>
/// Guest data transfer object
/// </summary>
public class GuestDto
{
    public int Id { get; set; }
    public int CheckInId { get; set; }
    public int GuestNumber { get; set; }
    public string GuestName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? MobileNo { get; set; }
    public string? PanOrAadharNo { get; set; }
    public string? Photo1Path { get; set; }
    public string? Photo2Path { get; set; }
    public DateTime? ActualCheckInDate { get; set; }
    public DateTime? ActualCheckOutDate { get; set; }
    public int Status { get; set; }
    public string StatusName => ((GuestStatus)Status).ToString();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}

public enum GuestStatus
{
    Active = 0,
    CheckedOut = 1
}
