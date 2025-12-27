using HMSMini.API.Models.DTOs.Room;

namespace HMSMini.API.Services.Interfaces;

public interface IRoomService
{
    Task<List<RoomDto>> GetAllAsync();
    Task<RoomDto> GetByIdAsync(int id);
    Task<List<RoomDto>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut);
    Task<RoomDto> CreateAsync(CreateRoomDto dto);
    Task<RoomDto> UpdateStatusAsync(int id, UpdateRoomStatusDto dto);
    Task DeleteAsync(int id);
    Task<int> GetRoomIdByNumberAsync(string roomNumber);
}
