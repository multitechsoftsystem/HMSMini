namespace HMSMini.API.Models.DTOs.FinancialReport;

public class BusinessSourceOutstandingItemDto
{
    public int BusinessSourceId { get; set; }
    public string BusinessSourceName { get; set; } = string.Empty;
    public decimal TotalCharged { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal Outstanding { get; set; }
}

public class BusinessSourceOutstandingDto
{
    public List<BusinessSourceOutstandingItemDto> Sources { get; set; } = new();
    public decimal GrandTotalCharged { get; set; }
    public decimal GrandTotalPaid { get; set; }
    public decimal GrandTotalOutstanding { get; set; }
    public DateTime AsOfDate { get; set; }
}
