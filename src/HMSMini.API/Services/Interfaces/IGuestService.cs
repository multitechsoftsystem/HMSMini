using HMSMini.API.Models.DTOs.Guest;

namespace HMSMini.API.Services.Interfaces;

public interface IGuestService
{
    Task<GuestDto> GetByIdAsync(int id);
    Task<List<GuestDto>> GetByCheckInIdAsync(int checkInId);
    Task<GuestDto> CreateAsync(int checkInId, CreateGuestDto dto);
    Task<GuestDto> UpdateAsync(int id, CreateGuestDto dto);
    Task<GuestDto> UpdatePhotoPathAsync(int id, int photoNumber, string photoPath);
    Task<GuestDto> CheckOutGuestAsync(int id);
    Task DeleteAsync(int id);

    // Guest search methods
    Task<List<GuestDto>> SearchGuestsByMobileAsync(string mobile);
    Task<List<GuestDto>> SearchGuestsByNameAsync(string name);
    Task<List<GuestDto>> SearchGuestsAsync(string query);
}
