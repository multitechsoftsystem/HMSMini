using System.ComponentModel.DataAnnotations;

namespace HMSMini.API.Models.DTOs.Billing;

/// <summary>
/// DTO for finalizing checkout and creating invoice
/// </summary>
public class FinalizeInvoiceDto
{
    /// <summary>
    /// Optional: Explicit checkout date/time. If null, uses current DateTime
    /// </summary>
    public DateTime? ActualCheckOutDate { get; set; }

    /// <summary>
    /// Optional: Payment method (Cash, Card, UPI, etc.)
    /// </summary>
    [StringLength(50)]
    public string? PaymentMethod { get; set; }

    /// <summary>
    /// Optional: Payment status (Paid, Unpaid, PartiallyPaid)
    /// </summary>
    [StringLength(50)]
    public string PaymentStatus { get; set; } = "Unpaid";

    /// <summary>
    /// Optional: Payment date. If null and PaymentStatus is "Paid", uses current DateTime
    /// </summary>
    public DateTime? PaymentDate { get; set; }

    /// <summary>
    /// Optional: Payment notes
    /// </summary>
    [StringLength(1000)]
    public string? PaymentNotes { get; set; }
}
