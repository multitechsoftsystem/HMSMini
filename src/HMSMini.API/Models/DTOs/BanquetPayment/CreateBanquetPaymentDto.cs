using System.ComponentModel.DataAnnotations;
using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.DTOs.BanquetPayment;

public class CreateBanquetPaymentDto
{
    [Required]
    public DateTime PaymentDate { get; set; }

    [Required]
    public BanquetPaymentType PaymentType { get; set; }

    [Required]
    public BanquetPaymentMode PaymentMode { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    [StringLength(200)]
    public string? ReferenceNumber { get; set; }

    [StringLength(100)]
    public string? ReceivedBy { get; set; }
}
