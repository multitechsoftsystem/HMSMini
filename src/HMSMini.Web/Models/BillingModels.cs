namespace HMSMini.Web.Models;

// Daily charge model
public class DailyChargeModel
{
    public DateTime Date { get; set; }
    public decimal RoomRate { get; set; }
    public decimal MealPlanRate { get; set; }
    public decimal SubtotalBeforeDiscount { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal SubtotalAfterDiscount { get; set; }
    public List<TaxLineModel> Taxes { get; set; } = new();
    public decimal TotalTax { get; set; }
    public decimal DayTotal { get; set; }
    public string? RateChangeNote { get; set; }
}

// Tax line model
public class TaxLineModel
{
    public string TaxType { get; set; } = string.Empty;
    public decimal TaxPercentage { get; set; }
    public decimal TaxableAmount { get; set; }
    public decimal TaxAmount { get; set; }
}

// Tax summary model
public class TaxSummaryModel
{
    public string TaxType { get; set; } = string.Empty;
    public decimal TaxPercentage { get; set; }
    public decimal TotalTaxAmount { get; set; }
}

// Additional charge model
public class AdditionalChargeModel
{
    public int Id { get; set; }
    public int CheckInId { get; set; }
    public DateTime ChargeDate { get; set; }
    public string ChargeType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public string? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Create additional charge model
public class CreateAdditionalChargeModel
{
    public DateTime ChargeDate { get; set; } = DateTime.Today;
    public string ChargeType { get; set; } = "Room Service";
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Quantity { get; set; } = 1;
}

// Update additional charge model
public class UpdateAdditionalChargeModel
{
    public DateTime ChargeDate { get; set; }
    public string ChargeType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Quantity { get; set; }
}

// Bill preview model
public class BillPreviewModel
{
    public int CheckInId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string GuestNames { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public DateTime ActualCheckInDate { get; set; }
    public DateTime ProposedCheckOutDate { get; set; }
    public int TotalNights { get; set; }
    public int Pax { get; set; }
    public List<DailyChargeModel> DailyCharges { get; set; } = new();
    public List<AdditionalChargeModel> AdditionalCharges { get; set; } = new();
    public decimal RoomChargesSubtotal { get; set; }
    public decimal MealChargesSubtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal AdditionalChargesSubtotal { get; set; }
    public decimal SubtotalBeforeTax { get; set; }
    public List<TaxSummaryModel> TaxBreakdown { get; set; } = new();
    public decimal TotalTax { get; set; }
    public decimal GrandTotal { get; set; }
    public DateTime GeneratedAt { get; set; }
    public string? GeneratedBy { get; set; }
}

// Invoice model
public class InvoiceModel
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public int CheckInId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string GuestNames { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public DateTime ActualCheckInDate { get; set; }
    public DateTime ActualCheckOutDate { get; set; }
    public int TotalNights { get; set; }
    public int Pax { get; set; }
    public List<DailyChargeModel> DailyCharges { get; set; } = new();
    public List<AdditionalChargeModel> AdditionalCharges { get; set; } = new();
    public decimal RoomChargesSubtotal { get; set; }
    public decimal MealChargesSubtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal AdditionalChargesSubtotal { get; set; }
    public decimal SubtotalBeforeTax { get; set; }
    public List<TaxSummaryModel> TaxBreakdown { get; set; } = new();
    public decimal TotalTax { get; set; }
    public decimal GrandTotal { get; set; }
    public string PaymentStatus { get; set; } = "Unpaid";
    public string? PaymentMethod { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string? PaymentNotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}

// Finalize invoice model
public class FinalizeInvoiceModel
{
    public DateTime? ActualCheckOutDate { get; set; }
    public string? PaymentMethod { get; set; }
    public string PaymentStatus { get; set; } = "Unpaid";
    public DateTime? PaymentDate { get; set; }
    public string? PaymentNotes { get; set; }
}
