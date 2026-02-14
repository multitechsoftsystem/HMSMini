using HMSMini.API.Models.DTOs.MenuItem;

namespace HMSMini.API.Models.DTOs.MenuPackage;

public class MenuPackageDetailDto
{
    public int Id { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public decimal RatePerPlate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<MenuItemDto> Items { get; set; } = new();
}
