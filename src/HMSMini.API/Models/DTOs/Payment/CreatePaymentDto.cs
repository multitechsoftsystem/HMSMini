using System.ComponentModel.DataAnnotations;
using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.DTOs.Payment;

public class CreatePaymentDto
{
    [Required]
    public PaymentSourceType SourceType { get; set; }

    public int? CheckInId { get; set; }

    public int? BanquetBookingId { get; set; }

    [Required]
    public DateTime PaymentDate { get; set; }

    [Required]
    public PaymentType PaymentType { get; set; }

    [Required]
    public PaymentMode PaymentMode { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    [StringLength(200)]
    public string? ReferenceNumber { get; set; }

    [StringLength(100)]
    public string? ReceivedBy { get; set; }

    [StringLength(1000)]
    public string? Remarks { get; set; }
}
