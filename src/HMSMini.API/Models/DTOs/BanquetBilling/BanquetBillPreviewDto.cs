using HMSMini.API.Models.DTOs.Billing;

namespace HMSMini.API.Models.DTOs.BanquetBilling;

public class BanquetBillPreviewDto
{
    public int BanquetBookingId { get; set; }
    public string BookingNumber { get; set; } = string.Empty;
    public string HallName { get; set; } = string.Empty;
    public string EventTypeName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string ContactPersonName { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public int ExpectedGuests { get; set; }
    public int? ActualGuests { get; set; }

    // Charges breakdown
    public decimal HallRent { get; set; }
    public List<BanquetMenuChargeDto> MenuCharges { get; set; } = new();
    public decimal MenuChargesSubtotal { get; set; }
    public List<BanquetServiceChargeDto> ServiceCharges { get; set; } = new();
    public decimal ServiceChargesSubtotal { get; set; }
    public List<BanquetAdditionalChargeDto> AdditionalCharges { get; set; } = new();
    public decimal AdditionalChargesSubtotal { get; set; }

    // Totals
    public decimal Subtotal { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal SubtotalAfterDiscount { get; set; }
    public List<TaxSummaryDto> TaxBreakdown { get; set; } = new();
    public decimal TotalTax { get; set; }
    public decimal GrandTotal { get; set; }

    // Payment info
    public decimal TotalPaid { get; set; }
    public decimal BalanceDue { get; set; }

    public DateTime GeneratedAt { get; set; }
}
