namespace HMSMini.Web.Models;

// Enums for Accounting
public enum AccountType
{
    Asset = 1,
    Liability = 2,
    Equity = 3,
    Income = 4,
    Expense = 5
}

public enum JournalSourceType
{
    DayClosing = 0,
    ExpenseVoucher = 1,
    PaymentVoucher = 2,
    Receipt = 3,
    GuestPayment = 4,
    Manual = 5
}

// Financial Year
public class FinancialYearModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public bool IsClosed { get; set; }
    public DateTime? ClosedAt { get; set; }
    public string? ClosedBy { get; set; }
}

public class CreateFinancialYearModel
{
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsCurrent { get; set; }
}

// Chart of Accounts
public class ChartOfAccountModel
{
    public int Id { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public string AccountTypeName { get; set; } = string.Empty;
    public int? ParentAccountId { get; set; }
    public string? ParentAccountName { get; set; }
    public bool IsSystemAccount { get; set; }
    public bool IsActive { get; set; }
}

public class CreateChartOfAccountModel
{
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public int? ParentAccountId { get; set; }
}

public class UpdateChartOfAccountModel
{
    public string AccountName { get; set; } = string.Empty;
    public int? ParentAccountId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class AccountDropdownModel
{
    public int Id { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public string DisplayName => $"{AccountCode} - {AccountName}";
}

// Journal Entry
public class JournalEntryModel
{
    public int Id { get; set; }
    public string EntryNumber { get; set; } = string.Empty;
    public DateTime EntryDate { get; set; }
    public int FinancialYearId { get; set; }
    public string? FinancialYearName { get; set; }
    public string Description { get; set; } = string.Empty;
    public JournalSourceType SourceType { get; set; }
    public string SourceTypeName { get; set; } = string.Empty;
    public int? SourceId { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsReversed { get; set; }
    public int? ReversalOfId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public List<JournalEntryLineModel> Lines { get; set; } = new();
}

public class JournalEntryLineModel
{
    public int Id { get; set; }
    public int JournalEntryId { get; set; }
    public int AccountId { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public string? Description { get; set; }
}

public class CreateJournalEntryModel
{
    public DateTime EntryDate { get; set; } = DateTime.Today;
    public string Description { get; set; } = string.Empty;
    public List<CreateJournalEntryLineModel> Lines { get; set; } = new();
}

public class CreateJournalEntryLineModel
{
    public int AccountId { get; set; }
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public string? Description { get; set; }
}

// Expense Head
public class ExpenseHeadModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? DefaultAccountId { get; set; }
    public string? DefaultAccountName { get; set; }
    public bool IsActive { get; set; }
}

public class CreateExpenseHeadModel
{
    public string Name { get; set; } = string.Empty;
    public int? DefaultAccountId { get; set; }
}

public class UpdateExpenseHeadModel
{
    public string Name { get; set; } = string.Empty;
    public int? DefaultAccountId { get; set; }
    public bool IsActive { get; set; } = true;
}

// Expense Voucher
public class ExpenseVoucherModel
{
    public int Id { get; set; }
    public string VoucherNumber { get; set; } = string.Empty;
    public DateTime VoucherDate { get; set; }
    public int FinancialYearId { get; set; }
    public int ExpenseHeadId { get; set; }
    public string ExpenseHeadName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? PaidTo { get; set; }
    public PaymentMode PaymentMode { get; set; }
    public int? BankAccountId { get; set; }
    public string? BankAccountName { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Narration { get; set; }
    public int? JournalEntryId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}

public class ExpenseVoucherListModel
{
    public int Id { get; set; }
    public string VoucherNumber { get; set; } = string.Empty;
    public DateTime VoucherDate { get; set; }
    public string ExpenseHeadName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? PaidTo { get; set; }
    public string PaymentModeName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateExpenseVoucherModel
{
    public DateTime VoucherDate { get; set; } = DateTime.Today;
    public int ExpenseHeadId { get; set; }
    public decimal Amount { get; set; }
    public string? PaidTo { get; set; }
    public PaymentMode PaymentMode { get; set; }
    public int? BankAccountId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Narration { get; set; }
}

// Payment Voucher
public class PaymentVoucherModel
{
    public int Id { get; set; }
    public string VoucherNumber { get; set; } = string.Empty;
    public DateTime VoucherDate { get; set; }
    public int FinancialYearId { get; set; }
    public string PayeeName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public PaymentMode PaymentMode { get; set; }
    public int? BankAccountId { get; set; }
    public string? BankAccountName { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Narration { get; set; }
    public int? ExpenseVoucherId { get; set; }
    public string? ExpenseVoucherNumber { get; set; }
    public int? JournalEntryId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}

public class PaymentVoucherListModel
{
    public int Id { get; set; }
    public string VoucherNumber { get; set; } = string.Empty;
    public DateTime VoucherDate { get; set; }
    public string PayeeName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentModeName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreatePaymentVoucherModel
{
    public DateTime VoucherDate { get; set; } = DateTime.Today;
    public string PayeeName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public PaymentMode PaymentMode { get; set; }
    public int? BankAccountId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Narration { get; set; }
    public int? ExpenseVoucherId { get; set; }
}

// Receipt
public class ReceiptModel
{
    public int Id { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public DateTime ReceiptDate { get; set; }
    public int FinancialYearId { get; set; }
    public string ReceivedFrom { get; set; } = string.Empty;
    public int? CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public decimal Amount { get; set; }
    public PaymentMode PaymentMode { get; set; }
    public int? BankAccountId { get; set; }
    public string? BankAccountName { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Narration { get; set; }
    public int? JournalEntryId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public List<ReceiptAllocationModel> Allocations { get; set; } = new();
}

public class ReceiptListModel
{
    public int Id { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public DateTime ReceiptDate { get; set; }
    public string ReceivedFrom { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public decimal Amount { get; set; }
    public string PaymentModeName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class ReceiptAllocationModel
{
    public int Id { get; set; }
    public int ReceiptId { get; set; }
    public int? InvoiceId { get; set; }
    public string? InvoiceNumber { get; set; }
    public int? BanquetInvoiceId { get; set; }
    public string? BanquetInvoiceNumber { get; set; }
    public decimal AllocatedAmount { get; set; }
}

public class CreateReceiptModel
{
    public DateTime ReceiptDate { get; set; } = DateTime.Today;
    public string ReceivedFrom { get; set; } = string.Empty;
    public int? CompanyId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMode PaymentMode { get; set; }
    public int? BankAccountId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Narration { get; set; }
    public List<CreateReceiptAllocationModel> Allocations { get; set; } = new();
}

public class CreateReceiptAllocationModel
{
    public int? InvoiceId { get; set; }
    public int? BanquetInvoiceId { get; set; }
    public decimal AllocatedAmount { get; set; }
}

public class OutstandingInvoiceModel
{
    public int InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public string? GuestNames { get; set; }
    public string? CompanyName { get; set; }
    public decimal GrandTotal { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal BalanceDue { get; set; }
    public string InvoiceType { get; set; } = string.Empty;
}

// Trial Balance
public class TrialBalanceModel
{
    public int? FinancialYearId { get; set; }
    public string? FinancialYearName { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public List<TrialBalanceLineModel> Lines { get; set; } = new();
}

public class TrialBalanceLineModel
{
    public int AccountId { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string AccountTypeName { get; set; } = string.Empty;
    public decimal OpeningDebit { get; set; }
    public decimal OpeningCredit { get; set; }
    public decimal PeriodDebit { get; set; }
    public decimal PeriodCredit { get; set; }
    public decimal ClosingDebit { get; set; }
    public decimal ClosingCredit { get; set; }
}

// Balance Sheet
public class BalanceSheetModel
{
    public DateTime AsOfDate { get; set; }
    public int? FinancialYearId { get; set; }
    public string? FinancialYearName { get; set; }
    public List<BalanceSheetLineModel> Assets { get; set; } = new();
    public List<BalanceSheetLineModel> Liabilities { get; set; } = new();
    public List<BalanceSheetLineModel> Equity { get; set; } = new();
    public decimal RetainedEarnings { get; set; }
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal TotalEquity { get; set; }
}

public class BalanceSheetLineModel
{
    public int AccountId { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}
