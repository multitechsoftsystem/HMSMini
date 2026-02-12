namespace HMSMini.API.Models.DTOs.Billing;

/// <summary>
/// Finalized invoice after checkout
/// </summary>
public class InvoiceDto
{
    // Invoice Information
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public int CheckInId { get; set; }

    // Guest & Room Information
    public string RoomNumber { get; set; } = string.Empty;
    public string GuestNames { get; set; } = string.Empty;
    public string? CompanyName { get; set; }

    // Stay Information
    public DateTime ActualCheckInDate { get; set; }
    public DateTime ActualCheckOutDate { get; set; }
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

    // Payment Tracking
    public decimal TotalPaid { get; set; }
    public decimal BalanceDue { get; set; }

    // Payment Information
    public string PaymentStatus { get; set; } = "Unpaid";
    public string? PaymentMethod { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string? PaymentNotes { get; set; }

    // Audit
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}
