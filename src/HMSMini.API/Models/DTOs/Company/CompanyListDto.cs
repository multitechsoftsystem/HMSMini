namespace HMSMini.API.Models.DTOs.Company;

/// <summary>
/// Lightweight DTO for listing companies
/// </summary>
public class CompanyListDto
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? GSTNumber { get; set; }
    public string? City { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactNumber { get; set; }
    public decimal DiscountPercentage { get; set; }
    public bool IsActive { get; set; }
    public int TotalCheckIns { get; set; }
}
