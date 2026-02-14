using System.ComponentModel.DataAnnotations;

namespace HMSMini.API.Models.DTOs.MenuCategory;

public class CreateMenuCategoryDto
{
    [Required(ErrorMessage = "Category name is required")]
    [StringLength(200)]
    public string CategoryName { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}
