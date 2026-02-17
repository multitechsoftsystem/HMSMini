using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.DTOs.Receipt;

public class ReceiptDto
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
    public string PaymentModeName => PaymentMode.ToString();
    public int? BankAccountId { get; set; }
    public string? BankAccountName { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Narration { get; set; }
    public int? JournalEntryId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public List<ReceiptAllocationDto> Allocations { get; set; } = new();
}

public class CreateReceiptDto
{
    public DateTime ReceiptDate { get; set; }
    public string ReceivedFrom { get; set; } = string.Empty;
    public int? CompanyId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMode PaymentMode { get; set; }
    public int? BankAccountId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Narration { get; set; }
    public List<CreateReceiptAllocationDto> Allocations { get; set; } = new();
}

public class ReceiptListDto
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

public class ReceiptAllocationDto
{
    public int Id { get; set; }
    public int ReceiptId { get; set; }
    public int? InvoiceId { get; set; }
    public string? InvoiceNumber { get; set; }
    public int? BanquetInvoiceId { get; set; }
    public string? BanquetInvoiceNumber { get; set; }
    public decimal AllocatedAmount { get; set; }
}

public class CreateReceiptAllocationDto
{
    public int? InvoiceId { get; set; }
    public int? BanquetInvoiceId { get; set; }
    public decimal AllocatedAmount { get; set; }
}

public class OutstandingInvoiceDto
{
    public int InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public string? GuestNames { get; set; }
    public string? CompanyName { get; set; }
    public decimal GrandTotal { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal BalanceDue { get; set; }
    public string InvoiceType { get; set; } = "Room"; // "Room" or "Banquet"
}
