using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.EventType;
using HMSMini.API.Models.Entities;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Services.Implementations;

public class EventTypeService : IEventTypeService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EventTypeService> _logger;

    public EventTypeService(ApplicationDbContext context, ILogger<EventTypeService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<EventTypeDto>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.EventTypes.Where(e => e.DeletedAt == null);
        if (!includeInactive)
            query = query.Where(e => e.IsActive);

        return await query.Select(e => new EventTypeDto
        {
            Id = e.Id,
            EventTypeName = e.EventTypeName,
            Description = e.Description,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt
        }).OrderBy(e => e.EventTypeName).ToListAsync();
    }

    public async Task<List<EventTypeDto>> GetActiveAsync()
    {
        return await _context.EventTypes
            .Where(e => e.IsActive && e.DeletedAt == null)
            .Select(e => new EventTypeDto
            {
                Id = e.Id,
                EventTypeName = e.EventTypeName,
                Description = e.Description,
                IsActive = e.IsActive,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            }).OrderBy(e => e.EventTypeName).ToListAsync();
    }

    public async Task<EventTypeDto?> GetByIdAsync(int id)
    {
        return await _context.EventTypes
            .Where(e => e.Id == id && e.DeletedAt == null)
            .Select(e => new EventTypeDto
            {
                Id = e.Id,
                EventTypeName = e.EventTypeName,
                Description = e.Description,
                IsActive = e.IsActive,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            }).FirstOrDefaultAsync();
    }

    public async Task<EventTypeDto> CreateAsync(CreateEventTypeDto dto)
    {
        var entity = new MEventType
        {
            EventTypeName = dto.EventTypeName,
            Description = dto.Description,
            IsActive = true
        };

        _context.EventTypes.Add(entity);
        await _context.SaveChangesAsync();
        return (await GetByIdAsync(entity.Id))!;
    }

    public async Task<EventTypeDto> UpdateAsync(int id, UpdateEventTypeDto dto)
    {
        var entity = await _context.EventTypes.FindAsync(id);
        if (entity == null || entity.DeletedAt != null)
            throw new NotFoundException(nameof(MEventType), id);

        entity.EventTypeName = dto.EventTypeName;
        entity.Description = dto.Description;

        await _context.SaveChangesAsync();
        return (await GetByIdAsync(id))!;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.EventTypes.FindAsync(id);
        if (entity == null || entity.DeletedAt != null)
            throw new NotFoundException(nameof(MEventType), id);

        entity.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<EventTypeDto> ActivateAsync(int id)
    {
        var entity = await _context.EventTypes.FindAsync(id);
        if (entity == null || entity.DeletedAt != null)
            throw new NotFoundException(nameof(MEventType), id);

        entity.IsActive = true;
        await _context.SaveChangesAsync();
        return (await GetByIdAsync(id))!;
    }

    public async Task<EventTypeDto> DeactivateAsync(int id)
    {
        var entity = await _context.EventTypes.FindAsync(id);
        if (entity == null || entity.DeletedAt != null)
            throw new NotFoundException(nameof(MEventType), id);

        entity.IsActive = false;
        await _context.SaveChangesAsync();
        return (await GetByIdAsync(id))!;
    }
}
