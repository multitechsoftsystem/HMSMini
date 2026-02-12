namespace HMSMini.API.Models.DTOs.FinancialReport;

public class CompanyOutstandingSummaryDto
{
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public decimal RoomCharges { get; set; }
    public decimal BanquetCharges { get; set; }
    public decimal TotalCharged { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal Outstanding { get; set; }
}
