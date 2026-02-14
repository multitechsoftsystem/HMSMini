using HMSMini.API.Models.DTOs.EventType;

namespace HMSMini.API.Services.Interfaces;

public interface IEventTypeService
{
    Task<List<EventTypeDto>> GetAllAsync(bool includeInactive = false);
    Task<List<EventTypeDto>> GetActiveAsync();
    Task<EventTypeDto?> GetByIdAsync(int id);
    Task<EventTypeDto> CreateAsync(CreateEventTypeDto dto);
    Task<EventTypeDto> UpdateAsync(int id, UpdateEventTypeDto dto);
    Task DeleteAsync(int id);
    Task<EventTypeDto> ActivateAsync(int id);
    Task<EventTypeDto> DeactivateAsync(int id);
}
