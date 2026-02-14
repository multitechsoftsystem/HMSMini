using System.ComponentModel.DataAnnotations;

namespace HMSMini.API.Models.DTOs.Voucher;

/// <summary>
/// DTO for cancelling a voucher
/// </summary>
public class CancelVoucherDto
{
    [Required]
    public string CancellationReason { get; set; } = string.Empty;

    public string? CancelledBy { get; set; }
}
