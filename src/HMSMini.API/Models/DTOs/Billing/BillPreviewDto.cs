namespace HMSMini.API.Models.DTOs.Billing;

/// <summary>
/// Complete bill preview before checkout
/// </summary>
public class BillPreviewDto
{
    // Check-in Information
    public int CheckInId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string GuestNames { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public DateTime ActualCheckInDate { get; set; }
    public DateTime ProposedCheckOutDate { get; set; }
    public int TotalNights { get; set; }
    public int Pax { get; set; }

    // Day-wise Breakdown
    public List<DailyChargeDto> DailyCharges { get; set; } = new();

    // Additional Charges
    public List<AdditionalChargeDto> AdditionalCharges { get; set; } = new();

    // Summary
    public decimal RoomChargesSubtotal { get; set; }
    public decimal MealChargesSubtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal AdditionalChargesSubtotal { get; set; }
    public decimal SubtotalBeforeTax { get; set; }

    // Tax Breakdown
    public List<TaxSummaryDto> TaxBreakdown { get; set; } = new();
    public decimal TotalTax { get; set; }

    // Grand Total
    public decimal GrandTotal { get; set; }

    // Metadata
    public DateTime GeneratedAt { get; set; }
    public string? GeneratedBy { get; set; }
}
