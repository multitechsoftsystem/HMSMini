using System.ComponentModel.DataAnnotations;
using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.DTOs.BanquetBooking;

public class UpdateBanquetBookingDto
{
    [Required]
    public int BanquetHallId { get; set; }

    [Required]
    public int EventTypeId { get; set; }

    [Required]
    public DateTime EventDate { get; set; }

    [Required]
    public TimeSpan EventStartTime { get; set; }

    [Required]
    public TimeSpan EventEndTime { get; set; }

    [Required]
    [Range(1, 100000)]
    public int ExpectedGuests { get; set; }

    public int? ActualGuests { get; set; }

    [Required]
    public BanquetPricingType PricingType { get; set; }

    [Required]
    [StringLength(200)]
    public string ContactPersonName { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string ContactPhone { get; set; } = string.Empty;

    public int? CompanyId { get; set; }

    public int? CheckInId { get; set; }

    public TaxType TaxType { get; set; }

    [Range(0, 100)]
    public decimal DiscountPercentage { get; set; }

    [Range(0, double.MaxValue)]
    public decimal HallRent { get; set; }

    [StringLength(1000)]
    public string? Remarks { get; set; }
}
