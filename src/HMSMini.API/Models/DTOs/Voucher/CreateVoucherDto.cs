using System.ComponentModel.DataAnnotations;

namespace HMSMini.API.Models.DTOs.Voucher;

/// <summary>
/// DTO for creating a new voucher
/// </summary>
public class CreateVoucherDto
{
    [Required]
    public string VoucherType { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
    public decimal Amount { get; set; }

    [Required]
    public int CheckInId { get; set; }

    public int? GuestId { get; set; }

    [Required]
    public string RoomNumber { get; set; } = string.Empty;

    public bool AutoPostDaily { get; set; } = false;

    public string? TaxType { get; set; }

    [Range(0, 100)]
    public decimal? TaxPercentage { get; set; }

    public decimal? TaxableAmount { get; set; }

    public int? AdditionalChargeId { get; set; }

    public string? PostedBy { get; set; }
}
