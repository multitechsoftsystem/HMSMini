namespace HMSMini.API.Models.DTOs.FinancialReport;

public class CompanyOutstandingListDto
{
    public List<CompanyOutstandingSummaryDto> Companies { get; set; } = new();
    public decimal GrandTotalCharged { get; set; }
    public decimal GrandTotalPaid { get; set; }
    public decimal GrandTotalOutstanding { get; set; }
    public DateTime AsOfDate { get; set; }
}
