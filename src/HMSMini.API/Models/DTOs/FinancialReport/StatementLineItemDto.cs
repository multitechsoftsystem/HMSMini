using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.DTOs.FinancialReport;

public class StatementLineItemDto
{
    public DateTime Date { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public PaymentSourceType SourceType { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal ChargeAmount { get; set; }
    public decimal PaymentAmount { get; set; }
    public decimal RunningBalance { get; set; }
}
