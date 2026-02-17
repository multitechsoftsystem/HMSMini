using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.DTOs.PaymentVoucher;

public class PaymentVoucherDto
{
    public int Id { get; set; }
    public string VoucherNumber { get; set; } = string.Empty;
    public DateTime VoucherDate { get; set; }
    public int FinancialYearId { get; set; }
    public string PayeeName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public PaymentMode PaymentMode { get; set; }
    public string PaymentModeName => PaymentMode.ToString();
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

public class CreatePaymentVoucherDto
{
    public DateTime VoucherDate { get; set; }
    public string PayeeName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public PaymentMode PaymentMode { get; set; }
    public int? BankAccountId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Narration { get; set; }
    public int? ExpenseVoucherId { get; set; }
}

public class PaymentVoucherListDto
{
    public int Id { get; set; }
    public string VoucherNumber { get; set; } = string.Empty;
    public DateTime VoucherDate { get; set; }
    public string PayeeName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentModeName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
