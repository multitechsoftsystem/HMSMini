using System.ComponentModel.DataAnnotations;
using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.DTOs.BanquetBooking;

public class UpdateBanquetBookingStatusDto
{
    [Required]
    public BanquetBookingStatus NewStatus { get; set; }
}
