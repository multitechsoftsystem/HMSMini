using HMSMini.API.Models.DTOs.BanquetHall;

namespace HMSMini.API.Services.Interfaces;

public interface IBanquetHallService
{
    Task<List<BanquetHallDto>> GetAllAsync(bool includeInactive = false);
    Task<BanquetHallDto?> GetByIdAsync(int id);
    Task<BanquetHallDto> CreateAsync(CreateBanquetHallDto dto);
    Task<BanquetHallDto> UpdateAsync(int id, UpdateBanquetHallDto dto);
    Task DeleteAsync(int id);
    Task<BanquetHallDto> ActivateAsync(int id);
    Task<BanquetHallDto> DeactivateAsync(int id);
    Task<bool> CheckAvailabilityAsync(int hallId, DateTime date, TimeSpan startTime, TimeSpan endTime, int? excludeBookingId = null);
}
