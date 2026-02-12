using System.ComponentModel.DataAnnotations.Schema;

namespace HMSMini.API.Models.Entities;

/// <summary>
/// Accounting ledger entry for all hotel charges (Room Tariff, Meal Plan, Taxes, Additional Charges)
/// Vouchers are immutable once posted - corrections via reversing entries
/// </summary>
public class Voucher
{
    public int Id { get; set; }

    /// <summary>
    /// Unique voucher number for accounting reference (e.g., "VCH-20260127-0001")
    /// </summary>
    public string VoucherNumber { get; set; } = string.Empty;

    /// <summary>
    /// Business date when the voucher is recorded (same as working date)
    /// </summary>
    public DateTime VoucherDate { get; set; }

    /// <summary>
    /// Posting date for accounting (same as VoucherDate per requirements)
    /// </summary>
    public DateTime PostingDate { get; set; }

    /// <summary>
    /// Type of voucher: RoomTariff, MealPlan, Tax-CGST, Tax-SGST, Tax-IGST, AdditionalCharge, Discount, etc.
    /// </summary>
    public string VoucherType { get; set; } = string.Empty;

    /// <summary>
    /// Description of the charge
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Voucher amount (can be negative for reversals/discounts)
    /// </summary>
    [Column(TypeName = "decimal(10,2)")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Foreign key to the check-in this voucher belongs to (nullable for banquet vouchers)
    /// </summary>
    public int? CheckInId { get; set; }

    /// <summary>
    /// Foreign key to banquet booking (for banquet-related vouchers)
    /// </summary>
    public int? BanquetBookingId { get; set; }

    /// <summary>
    /// Foreign key to unified payment (for payment vouchers)
    /// </summary>
    public int? PaymentId { get; set; }

    /// <summary>
    /// Optional guest ID if voucher is specific to one guest
    /// </summary>
    public int? GuestId { get; set; }

    /// <summary>
    /// Room number for reference
    /// </summary>
    public string RoomNumber { get; set; } = string.Empty;

    /// <summary>
    /// Posting status: Posted, Cancelled
    /// </summary>
    public string PostingStatus { get; set; } = "Posted";

    /// <summary>
    /// If true, this voucher type is auto-posted during daily closing
    /// </summary>
    public bool AutoPostDaily { get; set; } = false;

    /// <summary>
    /// Tax type if this is a tax voucher (GST, CGST, SGST, IGST)
    /// </summary>
    public string? TaxType { get; set; }

    /// <summary>
    /// Tax percentage applied (e.g., 12.00 for 12%)
    /// </summary>
    [Column(TypeName = "decimal(5,2)")]
    public decimal? TaxPercentage { get; set; }

    /// <summary>
    /// Base amount on which tax was calculated
    /// </summary>
    [Column(TypeName = "decimal(10,2)")]
    public decimal? TaxableAmount { get; set; }

    /// <summary>
    /// Reference to additional charge if this voucher was created from an additional charge
    /// </summary>
    public int? AdditionalChargeId { get; set; }

    /// <summary>
    /// When this voucher was posted to the ledger
    /// </summary>
    public DateTime PostedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// User who posted this voucher
    /// </summary>
    public string? PostedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// If voucher is cancelled, when and by whom
    /// </summary>
    public DateTime? CancelledAt { get; set; }
    public string? CancelledBy { get; set; }
    public string? CancellationReason { get; set; }

    // Navigation properties
    [ForeignKey(nameof(CheckInId))]
    public virtual CheckIn? CheckIn { get; set; }

    [ForeignKey(nameof(BanquetBookingId))]
    public virtual BanquetBooking? BanquetBooking { get; set; }

    [ForeignKey(nameof(PaymentId))]
    public virtual Payment? Payment { get; set; }

    [ForeignKey(nameof(GuestId))]
    public virtual Guest? Guest { get; set; }

    [ForeignKey(nameof(AdditionalChargeId))]
    public virtual AdditionalCharge? AdditionalCharge { get; set; }
}
