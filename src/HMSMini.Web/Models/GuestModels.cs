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
