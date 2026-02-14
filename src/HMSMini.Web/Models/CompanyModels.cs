namespace HMSMini.Web.Models;

public class CompanyDto
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? GSTNumber { get; set; }
    public string? ContactPerson { get; set; }
    public string? Designation { get; set; }
    public string? Email { get; set; }
    public string? ContactNumber { get; set; }
    public decimal DiscountPercentage { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

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

public class CreateCompanyDto
{
    public string CompanyName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? GSTNumber { get; set; }
    public string? ContactPerson { get; set; }
    public string? Designation { get; set; }
    public string? Email { get; set; }
    public string? ContactNumber { get; set; }
    public decimal DiscountPercentage { get; set; }
}

public class UpdateCompanyDto
{
    public string CompanyName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? GSTNumber { get; set; }
    public string? ContactPerson { get; set; }
    public string? Designation { get; set; }
    public string? Email { get; set; }
    public string? ContactNumber { get; set; }
    public decimal DiscountPercentage { get; set; }
}
