namespace HMSMini.API.Models.DTOs.Voucher;

/// <summary>
/// Summary of vouchers by type
/// </summary>
public class VoucherSummaryDto
{
    public string VoucherType { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalAmount { get; set; }
}
