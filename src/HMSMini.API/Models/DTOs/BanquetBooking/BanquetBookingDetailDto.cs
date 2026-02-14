using HMSMini.API.Models.DTOs.BanquetBookingMenu;
using HMSMini.API.Models.DTOs.BanquetBookingService;
using HMSMini.API.Models.DTOs.BanquetCharge;
using HMSMini.API.Models.DTOs.BanquetPayment;
using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.DTOs.BanquetBooking;

public class BanquetBookingDetailDto
{
    public int Id { get; set; }
    public string BookingNumber { get; set; } = string.Empty;
    public int BanquetHallId { get; set; }
    public string HallName { get; set; } = string.Empty;
    public int EventTypeId { get; set; }
    public string EventTypeName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public TimeSpan EventStartTime { get; set; }
    public TimeSpan EventEndTime { get; set; }
    public int ExpectedGuests { get; set; }
    public int? ActualGuests { get; set; }
    public BanquetBookingStatus Status { get; set; }
    public BanquetPricingType PricingType { get; set; }
    public string ContactPersonName { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public int? CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public int? CheckInId { get; set; }
    public TaxType TaxType { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal HallRent { get; set; }
    public string? Remarks { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public List<BanquetBookingMenuDto> Menus { get; set; } = new();
    public List<BanquetBookingServiceDto> Services { get; set; } = new();
    public List<BanquetChargeDto> Charges { get; set; } = new();
    public List<BanquetPaymentDto> Payments { get; set; } = new();
}
