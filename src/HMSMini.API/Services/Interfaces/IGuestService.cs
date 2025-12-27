using HMSMini.API.Models.DTOs.Guest;

namespace HMSMini.API.Services.Interfaces;

public interface IGuestService
{
    Task<GuestDto> GetByIdAsync(int id);
    Task<List<GuestDto>> GetByCheckInIdAsync(int checkInId);
    Task<GuestDto> UpdateAsync(int id, CreateGuestDto dto);
    Task<GuestDto> UpdatePhotoPathAsync(int id, int photoNumber, string photoPath);
    Task DeleteAsync(int id);
}
