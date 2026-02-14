using System.ComponentModel.DataAnnotations;
using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.DTOs.MenuItem;

public class UpdateMenuItemDto
{
    [Required]
    public int MenuCategoryId { get; set; }

    [Required(ErrorMessage = "Item name is required")]
    [StringLength(200)]
    public string ItemName { get; set; } = string.Empty;

    [Required]
    public MenuItemType ItemType { get; set; }

    [Range(0, double.MaxValue)]
    public decimal PricePerPlate { get; set; }
}
