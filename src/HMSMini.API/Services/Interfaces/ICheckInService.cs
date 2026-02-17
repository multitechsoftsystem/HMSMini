using HMSMini.API.Models.DTOs.CheckIn;

namespace HMSMini.API.Services.Interfaces;

public interface ICheckInService
{
    Task<CheckInWithGuestsDto> GetByIdAsync(int id);
    Task<List<CheckInDto>> GetAllAsync();
    Task<List<CheckInDto>> GetActiveCheckInsAsync();
    Task<CheckInWithGuestsDto> CreateCheckInAsync(CreateCheckInDto dto);
    Task<CheckInDto?> UpdateCheckInAsync(int id, UpdateCheckInDto dto);
    Task<CheckInDto> ExtendStayAsync(int id, ExtendStayDto dto);
    Task CheckOutAsync(int id);
    Task DeleteAsync(int id);
    Task<CheckInWithGuestsDto> CreateSharedCheckInAsync(int existingCheckInId, ShareRoomDto dto);
    Task<int> GetActiveCheckInCountForRoomAsync(int roomId);
    Task<bool> IsRoomSharingEnabledAsync();
}
