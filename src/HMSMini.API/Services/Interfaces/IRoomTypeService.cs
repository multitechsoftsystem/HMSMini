using HMSMini.API.Models.DTOs.RoomType;

namespace HMSMini.API.Services.Interfaces;

public interface IRoomTypeService
{
    Task<List<RoomTypeDto>> GetAllAsync();
    Task<RoomTypeDto> GetByIdAsync(int id);
    Task<RoomTypeDto> CreateAsync(CreateRoomTypeDto dto);
    Task<RoomTypeDto> UpdateAsync(int id, UpdateRoomTypeDto dto);
    Task DeleteAsync(int id);
}
