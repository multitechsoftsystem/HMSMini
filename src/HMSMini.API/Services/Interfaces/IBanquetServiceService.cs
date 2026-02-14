using HMSMini.API.Models.DTOs.BanquetService;

namespace HMSMini.API.Services.Interfaces;

public interface IBanquetServiceService
{
    Task<List<BanquetServiceDto>> GetAllAsync(bool includeInactive = false);
    Task<List<BanquetServiceDto>> GetActiveAsync();
    Task<BanquetServiceDto?> GetByIdAsync(int id);
    Task<BanquetServiceDto> CreateAsync(CreateBanquetServiceDto dto);
    Task<BanquetServiceDto> UpdateAsync(int id, UpdateBanquetServiceDto dto);
    Task DeleteAsync(int id);
    Task<BanquetServiceDto> ActivateAsync(int id);
    Task<BanquetServiceDto> DeactivateAsync(int id);
}
