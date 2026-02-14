namespace HMSMini.Web.Models;

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
}

public enum GuestStatus
{
    Active = 0,
    CheckedOut = 1
}

public class CreateGuestDto
{
    public string GuestName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? MobileNo { get; set; }
    public string? PanOrAadharNo { get; set; }
}

public class GuestInfoDto
{
    public string GuestName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? MobileNo { get; set; }
    public string? IdType { get; set; }
    public string? IdNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
}

public class OcrReviewDto
{
    public string RawText { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public GuestInfoDto GuestInfo { get; set; } = new();
}
