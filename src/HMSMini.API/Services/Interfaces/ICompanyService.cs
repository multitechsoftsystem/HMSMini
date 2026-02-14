using HMSMini.API.Models.DTOs.Company;

namespace HMSMini.API.Services.Interfaces;

/// <summary>
/// Service interface for company management
/// </summary>
public interface ICompanyService
{
    Task<List<CompanyListDto>> GetAllAsync(bool includeInactive = false);
    Task<List<CompanyDto>> GetActiveCompaniesAsync();
    Task<CompanyDto?> GetByIdAsync(int id);
    Task<CompanyDto> CreateAsync(CreateCompanyDto dto);
    Task<CompanyDto> UpdateAsync(int id, UpdateCompanyDto dto);
    Task DeleteAsync(int id);
    Task<CompanyDto> ActivateAsync(int id);
    Task<CompanyDto> DeactivateAsync(int id);
    Task<List<CompanyDto>> SearchAsync(string searchTerm);
}
