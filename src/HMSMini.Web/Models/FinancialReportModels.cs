namespace HMSMini.Web.Models;

// Company Outstanding
public class CompanyOutstandingListModel
{
    public List<CompanyOutstandingSummaryModel> Companies { get; set; } = new();
    public decimal GrandTotalCharged { get; set; }
    public decimal GrandTotalPaid { get; set; }
    public decimal GrandTotalOutstanding { get; set; }
    public DateTime AsOfDate { get; set; }
}

public class CompanyOutstandingSummaryModel
{
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public decimal RoomCharges { get; set; }
    public decimal BanquetCharges { get; set; }
    public decimal TotalCharged { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal Outstanding { get; set; }
}

// Company Statement
public class CompanyStatementModel
{
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal TotalCharges { get; set; }
    public decimal TotalPayments { get; set; }
    public decimal ClosingBalance { get; set; }
    public List<StatementLineItemModel> LineItems { get; set; } = new();
}

public class StatementLineItemModel
{
    public DateTime Date { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public PaymentSourceType SourceType { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal ChargeAmount { get; set; }
    public decimal PaymentAmount { get; set; }
    public decimal RunningBalance { get; set; }
}

// Aging Report
public class AgingReportModel
{
    public List<AgingBucketModel> Companies { get; set; } = new();
    public decimal TotalCurrent { get; set; }
    public decimal TotalDays31To60 { get; set; }
    public decimal TotalDays61To90 { get; set; }
    public decimal TotalOver90Days { get; set; }
    public decimal GrandTotal { get; set; }
    public DateTime AsOfDate { get; set; }
}

public class AgingBucketModel
{
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public decimal Current { get; set; }
    public decimal Days31To60 { get; set; }
    public decimal Days61To90 { get; set; }
    public decimal Over90Days { get; set; }
    public decimal TotalOutstanding { get; set; }
}

// Business Source Outstanding
public class BusinessSourceOutstandingModel
{
    public List<BusinessSourceOutstandingItemModel> Sources { get; set; } = new();
    public decimal GrandTotalCharged { get; set; }
    public decimal GrandTotalPaid { get; set; }
    public decimal GrandTotalOutstanding { get; set; }
    public DateTime AsOfDate { get; set; }
}

public class BusinessSourceOutstandingItemModel
{
    public int BusinessSourceId { get; set; }
    public string BusinessSourceName { get; set; } = string.Empty;
    public decimal TotalCharged { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal Outstanding { get; set; }
}
