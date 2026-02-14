using System.ComponentModel.DataAnnotations;

namespace HMSMini.API.Models.DTOs.Company;

/// <summary>
/// DTO for creating a new company
/// </summary>
public class CreateCompanyDto
{
    [Required(ErrorMessage = "Company name is required")]
    [StringLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Address { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(100)]
    public string? State { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    [StringLength(50)]
    public string? GSTNumber { get; set; }

    [StringLength(200)]
    public string? ContactPerson { get; set; }

    [StringLength(100)]
    public string? Designation { get; set; }

    [EmailAddress]
    [StringLength(255)]
    public string? Email { get; set; }

    [StringLength(20)]
    public string? ContactNumber { get; set; }

    [Range(0, 100, ErrorMessage = "Discount percentage must be between 0 and 100")]
    public decimal DiscountPercentage { get; set; } = 0;
}
