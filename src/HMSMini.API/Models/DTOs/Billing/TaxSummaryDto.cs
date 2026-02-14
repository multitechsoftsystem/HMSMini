namespace HMSMini.API.Models.DTOs.Billing;

/// <summary>
/// Represents aggregated tax summary
/// </summary>
public class TaxSummaryDto
{
    public string TaxType { get; set; } = string.Empty; // e.g., "CGST", "SGST"
    public decimal TaxPercentage { get; set; }
    public decimal TotalTaxAmount { get; set; }
}
