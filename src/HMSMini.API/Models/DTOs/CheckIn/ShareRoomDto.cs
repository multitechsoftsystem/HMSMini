using System.ComponentModel.DataAnnotations;
using HMSMini.API.Models.DTOs.Guest;
using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.DTOs.CheckIn;

/// <summary>
/// DTO for creating a shared check-in in the same room as an existing check-in
/// </summary>
public class ShareRoomDto
{
    [Required]
    public DateTime CheckInDate { get; set; }

    [Required]
    public DateTime CheckOutDate { get; set; }

    public DateTime? ActualCheckInDate { get; set; }

    [StringLength(50)]
    public string? RegistrationNo { get; set; }

    [StringLength(1000)]
    public string? Remarks { get; set; }

    public int? CompanyId { get; set; }

    public int? BusinessSourceId { get; set; }

    public int? MealPlanId { get; set; }

    public int? GuestTypeId { get; set; }

    /// <summary>
    /// Tax type for this check-in (CGST+SGST or IGST)
    /// </summary>
    public TaxType TaxType { get; set; } = TaxType.CgstSgst;

    [Required]
    [MinLength(1)]
    [MaxLength(3)]
    public List<CreateGuestDto> Guests { get; set; } = new();
}
