namespace HMSMini.API.Models.DTOs.Reservation;

/// <summary>
/// DTO for extending a reservation's checkout date
/// </summary>
public class ExtendStayDto
{
    /// <summary>
    /// New checkout date (must be after current checkout date)
    /// </summary>
    public DateTime NewCheckOutDate { get; set; }
}
