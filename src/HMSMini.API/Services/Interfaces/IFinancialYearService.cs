using HMSMini.API.Models.DTOs.FinancialYear;

namespace HMSMini.API.Services.Interfaces;

public interface IFinancialYearService
{
    Task<List<FinancialYearDto>> GetAllAsync();
    Task<FinancialYearDto> GetByIdAsync(int id);
    Task<FinancialYearDto?> GetCurrentAsync();
    Task<FinancialYearDto> CreateAsync(CreateFinancialYearDto dto);
    Task SetCurrentAsync(int id);
    Task CloseAsync(int id, string? closedBy = null);
    Task<int> GetCurrentFinancialYearIdAsync();
}
