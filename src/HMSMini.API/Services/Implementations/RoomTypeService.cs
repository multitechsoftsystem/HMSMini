using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.RoomType;
using HMSMini.API.Models.Entities;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Services.Implementations;

public class RoomTypeService : IRoomTypeService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RoomTypeService> _logger;

    public RoomTypeService(ApplicationDbContext context, ILogger<RoomTypeService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<RoomTypeDto>> GetAllAsync()
    {
        return await _context.RoomTypes
            .Select(rt => new RoomTypeDto
            {
                RoomTypeId = rt.RoomTypeId,
                RoomType = rt.RoomType,
                RoomDescription = rt.RoomDescription
            })
            .ToListAsync();
    }

    public async Task<RoomTypeDto> GetByIdAsync(int id)
    {
        var roomType = await _context.RoomTypes.FindAsync(id);
        if (roomType == null)
            throw new NotFoundException(nameof(MRoomType), id);

        return new RoomTypeDto
        {
            RoomTypeId = roomType.RoomTypeId,
            RoomType = roomType.RoomType,
            RoomDescription = roomType.RoomDescription
        };
    }

    public async Task<RoomTypeDto> CreateAsync(CreateRoomTypeDto dto)
    {
        // Check if room type already exists
        if (await _context.RoomTypes.AnyAsync(rt => rt.RoomType == dto.RoomType))
            throw new BusinessRuleException($"Room type '{dto.RoomType}' already exists.");

        var roomType = new MRoomType
        {
            RoomType = dto.RoomType,
            RoomDescription = dto.RoomDescription
        };

        _context.RoomTypes.Add(roomType);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created room type {RoomType} with ID {Id}", roomType.RoomType, roomType.RoomTypeId);

        return new RoomTypeDto
        {
            RoomTypeId = roomType.RoomTypeId,
            RoomType = roomType.RoomType,
            RoomDescription = roomType.RoomDescription
        };
    }

    public async Task<RoomTypeDto> UpdateAsync(int id, UpdateRoomTypeDto dto)
    {
        var roomType = await _context.RoomTypes.FindAsync(id);
        if (roomType == null)
            throw new NotFoundException(nameof(MRoomType), id);

        // Check if new name conflicts with existing
        if (await _context.RoomTypes.AnyAsync(rt => rt.RoomType == dto.RoomType && rt.RoomTypeId != id))
            throw new BusinessRuleException($"Room type '{dto.RoomType}' already exists.");

        roomType.RoomType = dto.RoomType;
        roomType.RoomDescription = dto.RoomDescription;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated room type ID {Id}", id);

        return new RoomTypeDto
        {
            RoomTypeId = roomType.RoomTypeId,
            RoomType = roomType.RoomType,
            RoomDescription = roomType.RoomDescription
        };
    }

    public async Task DeleteAsync(int id)
    {
        var roomType = await _context.RoomTypes.FindAsync(id);
        if (roomType == null)
            throw new NotFoundException(nameof(MRoomType), id);

        // Check if any rooms use this type
        if (await _context.Rooms.AnyAsync(r => r.RoomTypeId == id))
            throw new BusinessRuleException("Cannot delete room type that is in use by rooms.");

        _context.RoomTypes.Remove(roomType);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted room type ID {Id}", id);
    }
}
