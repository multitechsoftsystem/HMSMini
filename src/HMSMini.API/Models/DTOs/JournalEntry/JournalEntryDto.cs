using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.DTOs.JournalEntry;

public class JournalEntryDto
{
    public int Id { get; set; }
    public string EntryNumber { get; set; } = string.Empty;
    public DateTime EntryDate { get; set; }
    public int FinancialYearId { get; set; }
    public string? FinancialYearName { get; set; }
    public string? Description { get; set; }
    public JournalSourceType SourceType { get; set; }
    public string SourceTypeName => SourceType.ToString();
    public int? SourceId { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsReversed { get; set; }
    public int? ReversalOfId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public List<JournalEntryLineDto> Lines { get; set; } = new();
}

public class JournalEntryLineDto
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public string? Description { get; set; }
}

public class CreateJournalEntryDto
{
    public DateTime EntryDate { get; set; }
    public string? Description { get; set; }
    public List<CreateJournalEntryLineDto> Lines { get; set; } = new();
}

public class CreateJournalEntryLineDto
{
    public int AccountId { get; set; }
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public string? Description { get; set; }
}
