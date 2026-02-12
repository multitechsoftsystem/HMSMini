namespace HMSMini.API.Models.DTOs.FinancialReport;

public class AgingBucketDto
{
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public decimal Current { get; set; }
    public decimal Days31To60 { get; set; }
    public decimal Days61To90 { get; set; }
    public decimal Over90Days { get; set; }
    public decimal TotalOutstanding { get; set; }
}

public class AgingReportDto
{
    public List<AgingBucketDto> Companies { get; set; } = new();
    public decimal TotalCurrent { get; set; }
    public decimal TotalDays31To60 { get; set; }
    public decimal TotalDays61To90 { get; set; }
    public decimal TotalOver90Days { get; set; }
    public decimal GrandTotal { get; set; }
    public DateTime AsOfDate { get; set; }
}
