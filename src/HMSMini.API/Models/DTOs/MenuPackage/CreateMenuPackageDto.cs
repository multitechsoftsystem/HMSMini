using System.ComponentModel.DataAnnotations;

namespace HMSMini.API.Models.DTOs.MenuPackage;

public class CreateMenuPackageDto
{
    [Required(ErrorMessage = "Package name is required")]
    [StringLength(200)]
    public string PackageName { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal RatePerPlate { get; set; }

    public List<int> MenuItemIds { get; set; } = new();
}
