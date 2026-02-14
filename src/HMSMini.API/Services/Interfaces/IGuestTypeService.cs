using HMSMini.API.Models.DTOs.GuestType;

namespace HMSMini.API.Services.Interfaces;

public interface IGuestTypeService
{
    Task<List<GuestTypeDto>> GetAllAsync(bool includeInactive = false);
    Task<List<GuestTypeDto>> GetActiveAsync();
    Task<GuestTypeDto?> GetByIdAsync(int id);
    Task<GuestTypeDto> CreateAsync(CreateGuestTypeDto dto);
    Task<GuestTypeDto> UpdateAsync(int id, UpdateGuestTypeDto dto);
    Task DeleteAsync(int id);
    Task<GuestTypeDto> ActivateAsync(int id);
    Task<GuestTypeDto> DeactivateAsync(int id);
}
