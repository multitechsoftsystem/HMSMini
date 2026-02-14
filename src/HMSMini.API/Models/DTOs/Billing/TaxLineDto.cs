namespace HMSMini.API.Models.DTOs.Billing;

/// <summary>
/// Represents a single tax line item
/// </summary>
public class TaxLineDto
{
    public string TaxType { get; set; } = string.Empty; // e.g., "CGST", "SGST", "IGST"
    public decimal TaxPercentage { get; set; }
    public decimal TaxableAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public string? SlabRange { get; set; } // e.g., "₹0 - ₹7,499.99"
    public decimal? DailyAmount { get; set; } // Daily amount for reference
}
