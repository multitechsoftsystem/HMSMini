using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSMini.API.Models.Entities;

[Table("BanquetInvoices")]
public class BanquetInvoice
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string InvoiceNumber { get; set; } = string.Empty;

    [Required]
    public DateTime InvoiceDate { get; set; }

    [Required]
    public int BanquetBookingId { get; set; }

    // Snapshot fields
    [Required]
    [StringLength(200)]
    public string HallName { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string EventTypeName { get; set; } = string.Empty;

    [Required]
    public DateTime EventDate { get; set; }

    [Required]
    [StringLength(200)]
    public string ContactPersonName { get; set; } = string.Empty;

    [StringLength(200)]
    public string? CompanyName { get; set; }

    public int ExpectedGuests { get; set; }

    public int? ActualGuests { get; set; }

    // JSON historical storage
    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string MenuChargesJson { get; set; } = "[]";

    [Column(TypeName = "nvarchar(max)")]
    public string? ServiceChargesJson { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string? AdditionalChargesJson { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string TaxBreakdownJson { get; set; } = "[]";

    [Column(TypeName = "nvarchar(max)")]
    public string? PaymentHistoryJson { get; set; }

    // Amount breakdowns
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal HallRent { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal MenuChargesSubtotal { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal ServiceChargesSubtotal { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal AdditionalChargesSubtotal { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal DiscountAmount { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal SubtotalBeforeTax { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalTax { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal GrandTotal { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalPaid { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal BalanceDue { get; set; }

    [Required]
    [StringLength(50)]
    public string PaymentStatus { get; set; } = "Unpaid";

    // Audit fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    [StringLength(100)]
    public string? DeletedBy { get; set; }

    // Navigation properties
    [ForeignKey(nameof(BanquetBookingId))]
    public virtual BanquetBooking BanquetBooking { get; set; } = null!;
}
