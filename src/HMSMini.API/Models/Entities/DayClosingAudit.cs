using System.ComponentModel.DataAnnotations.Schema;

namespace HMSMini.API.Models.Entities;

/// <summary>
/// Audit trail for daily closing operations
/// Records summary of vouchers posted and check-ins active during day close
/// </summary>
public class DayClosingAudit
{
    public int Id { get; set; }

    /// <summary>
    /// The business date that was closed
    /// </summary>
    public DateTime ClosedDate { get; set; }

    /// <summary>
    /// The new working date after closing (ClosedDate + 1 day)
    /// </summary>
    public DateTime NextWorkingDate { get; set; }

    /// <summary>
    /// Number of active check-ins on the closed date
    /// </summary>
    public int TotalActiveCheckIns { get; set; } = 0;

    /// <summary>
    /// Number of vouchers posted during this day close
    /// </summary>
    public int TotalVouchersPosted { get; set; } = 0;

    /// <summary>
    /// Total revenue amount posted during this day close
    /// </summary>
    [Column(TypeName = "decimal(12,2)")]
    public decimal TotalRevenuePosted { get; set; } = 0;

    /// <summary>
    /// JSON summary of vouchers posted by type (RoomTariff: count/amount, MealPlan: count/amount, etc.)
    /// </summary>
    public string? VoucherSummaryJson { get; set; }

    /// <summary>
    /// JSON summary of check-ins active during closing
    /// </summary>
    public string? CheckInSummaryJson { get; set; }

    /// <summary>
    /// Status of the closing operation: Completed, Failed, PartiallyCompleted
    /// </summary>
    public string ClosingStatus { get; set; } = "Completed";

    /// <summary>
    /// Error log if closing failed or encountered issues
    /// </summary>
    public string? ErrorLog { get; set; }

    /// <summary>
    /// When the day close was executed
    /// </summary>
    public DateTime ClosedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// User who executed the day close
    /// </summary>
    public string? ClosedBy { get; set; }

    /// <summary>
    /// How long the day close operation took (in seconds)
    /// </summary>
    public int? DurationSeconds { get; set; }
}
