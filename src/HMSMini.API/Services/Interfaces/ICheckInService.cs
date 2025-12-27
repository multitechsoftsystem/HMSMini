using HMSMini.API.Models.DTOs.CheckIn;

namespace HMSMini.API.Services.Interfaces;

public interface ICheckInService
{
    Task<CheckInWithGuestsDto> GetByIdAsync(int id);
    Task<List<CheckInDto>> GetAllAsync();
    Task<List<CheckInDto>> GetActiveCheckInsAsync();
    Task<CheckInWithGuestsDto> CreateCheckInAsync(CreateCheckInDto dto);
    Task CheckOutAsync(int id);
    Task DeleteAsync(int id);
}
