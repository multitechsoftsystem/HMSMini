namespace HMSMini.API.Models.DTOs.Billing;

/// <summary>
/// Represents an additional charge (room service, minibar, etc.)
/// </summary>
public class AdditionalChargeDto
{
    public int Id { get; set; }
    public int CheckInId { get; set; }
    public DateTime ChargeDate { get; set; }
    public string ChargeType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public bool ApplyTax { get; set; }
    public int? VoucherTaxConfigId { get; set; }
    public string? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
