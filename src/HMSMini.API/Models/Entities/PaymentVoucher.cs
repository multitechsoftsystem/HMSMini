using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.Entities;

[Table("PaymentVouchers")]
public class PaymentVoucher
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string VoucherNumber { get; set; } = string.Empty;

    [Required]
    public DateTime VoucherDate { get; set; }

    [Required]
    public int FinancialYearId { get; set; }

    [Required]
    [StringLength(200)]
    public string PayeeName { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(12,2)")]
    public decimal Amount { get; set; }

    [Required]
    public PaymentMode PaymentMode { get; set; }

    public int? BankAccountId { get; set; }

    [StringLength(200)]
    public string? ReferenceNumber { get; set; }

    [StringLength(1000)]
    public string? Narration { get; set; }

    public int? ExpenseVoucherId { get; set; }

    public int? JournalEntryId { get; set; }

    // Audit fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    [StringLength(100)]
    public string? DeletedBy { get; set; }

    // Navigation properties
    [ForeignKey(nameof(FinancialYearId))]
    public virtual FinancialYear FinancialYear { get; set; } = null!;

    [ForeignKey(nameof(BankAccountId))]
    public virtual ChartOfAccount? BankAccount { get; set; }

    [ForeignKey(nameof(ExpenseVoucherId))]
    public virtual ExpenseVoucher? ExpenseVoucher { get; set; }

    [ForeignKey(nameof(JournalEntryId))]
    public virtual JournalEntry? JournalEntry { get; set; }
}
