namespace HMSMini.API.Models.DTOs.MenuPackage;

public class MenuPackageDto
{
    public int Id { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public decimal RatePerPlate { get; set; }
    public bool IsActive { get; set; }
    public int ItemCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
