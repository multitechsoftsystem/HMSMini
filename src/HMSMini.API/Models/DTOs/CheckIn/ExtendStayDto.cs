namespace HMSMini.API.Models.DTOs.CheckIn;

/// <summary>
/// DTO for extending a guest's stay
/// </summary>
public class ExtendStayDto
{
    /// <summary>
    /// New checkout date (must be after current checkout date)
    /// </summary>
    public DateTime NewCheckOutDate { get; set; }
}
