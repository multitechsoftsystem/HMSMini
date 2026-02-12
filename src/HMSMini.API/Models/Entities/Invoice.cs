using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSMini.API.Models.Entities;

/// <summary>
/// Permanent invoice records for checked-out guests
/// </summary>
[Table("Invoices")]
public class Invoice
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Unique invoice number (format: INV-YYYY-NNNNN)
    /// </summary>
    [Required]
    [StringLength(50)]
    public string InvoiceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when invoice was generated
    /// </summary>
    [Required]
    public DateTime InvoiceDate { get; set; }

    /// <summary>
    /// Foreign key to check-in (one-to-one relationship)
    /// </summary>
    [Required]
    public int CheckInId { get; set; }

    // Guest & Room Info (snapshot at checkout time)

    [Required]
    [StringLength(10)]
    public string RoomNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string GuestNames { get; set; } = string.Empty;

    [StringLength(200)]
    public string? CompanyName { get; set; }

    // Stay Information

    [Required]
    public DateTime ActualCheckInDate { get; set; }

    [Required]
    public DateTime ActualCheckOutDate { get; set; }

    [Required]
    public int TotalNights { get; set; }

    // Charges Breakdown (stored as JSON for historical preservation)

    /// <summary>
    /// JSON array of daily charges (DailyChargeDto[])
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string DailyChargesJson { get; set; } = "[]";

    /// <summary>
    /// JSON array of additional charges (AdditionalChargeDto[])
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? AdditionalChargesJson { get; set; }

    /// <summary>
    /// JSON array of tax breakdown (TaxSummaryDto[])
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string TaxBreakdownJson { get; set; } = "[]";

    // Summary Amounts

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal RoomChargesSubtotal { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal MealChargesSubtotal { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal DiscountAmount { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal AdditionalChargesSubtotal { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal SubtotalBeforeTax { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalTax { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal GrandTotal { get; set; }

    // Payment Tracking

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalPaid { get; set; } = 0;

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal BalanceDue { get; set; } = 0;

    // Payment Information

    [Required]
    [StringLength(50)]
    public string PaymentStatus { get; set; } = "Unpaid";

    [StringLength(50)]
    public string? PaymentMethod { get; set; }

    public DateTime? PaymentDate { get; set; }

    [StringLength(1000)]
    public string? PaymentNotes { get; set; }

    // Audit fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    // Soft delete
    public DateTime? DeletedAt { get; set; }

    [StringLength(100)]
    public string? DeletedBy { get; set; }

    // Navigation properties
    [ForeignKey(nameof(CheckInId))]
    public virtual CheckIn CheckIn { get; set; } = null!;
}
