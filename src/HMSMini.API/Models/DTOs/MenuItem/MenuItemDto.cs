using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.DTOs.MenuItem;

public class MenuItemDto
{
    public int Id { get; set; }
    public int MenuCategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public MenuItemType ItemType { get; set; }
    public decimal PricePerPlate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
