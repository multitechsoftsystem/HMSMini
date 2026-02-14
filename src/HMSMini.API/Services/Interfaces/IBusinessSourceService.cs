using HMSMini.API.Models.DTOs.BusinessSource;

namespace HMSMini.API.Services.Interfaces;

/// <summary>
/// Service interface for business source operations
/// </summary>
public interface IBusinessSourceService
{
    Task<List<BusinessSourceDto>> GetAllAsync(bool includeInactive = false);
    Task<List<BusinessSourceDto>> GetActiveBusinessSourcesAsync();
    Task<BusinessSourceDto?> GetByIdAsync(int id);
    Task<BusinessSourceDto> CreateAsync(CreateBusinessSourceDto dto);
    Task<BusinessSourceDto> UpdateAsync(int id, UpdateBusinessSourceDto dto);
    Task DeleteAsync(int id);
    Task<BusinessSourceDto> ActivateAsync(int id);
    Task<BusinessSourceDto> DeactivateAsync(int id);
}
