namespace HMSMini.API.Models.DTOs.BanquetBookingMenu;

public class BanquetBookingMenuDto
{
    public int Id { get; set; }
    public int BanquetBookingId { get; set; }
    public int? MenuPackageId { get; set; }
    public string? PackageName { get; set; }
    public int? MenuItemId { get; set; }
    public string? ItemName { get; set; }
    public int Quantity { get; set; }
    public decimal RatePerPlate { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}
