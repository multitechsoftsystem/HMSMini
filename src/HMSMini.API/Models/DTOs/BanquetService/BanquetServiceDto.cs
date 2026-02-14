namespace HMSMini.API.Models.DTOs.BanquetService;

public class BanquetServiceDto
{
    public int Id { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public decimal DefaultRate { get; set; }
    public string? Unit { get; set; }
    public bool ApplyTax { get; set; }
    public int? VoucherTaxConfigId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
