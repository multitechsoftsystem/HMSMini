namespace HMSMini.API.Models.DTOs.Guest;

/// <summary>
/// Guest information extracted from OCR
/// </summary>
public class GuestInfoDto
{
    public string GuestName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? MobileNo { get; set; }
    public string? IdType { get; set; } // Aadhaar, PAN, DL
    public string? IdNumber { get; set; }
}
