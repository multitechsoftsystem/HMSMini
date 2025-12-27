using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.Room;
using HMSMini.API.Models.Entities;
using HMSMini.API.Models.Enums;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Services.Implementations;

public class RoomService : IRoomService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RoomService> _logger;

    public RoomService(ApplicationDbContext context, ILogger<RoomService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<RoomDto>> GetAllAsync()
    {
        return await _context.Rooms
            .Include(r => r.RoomType)
            .Select(r => new RoomDto
            {
                RoomId = r.RoomId,
                RoomNumber = r.RoomNumber,
                RoomTypeId = r.RoomTypeId,
                RoomTypeName = r.RoomType.RoomType,
                RoomStatus = r.RoomStatus,
                RoomStatusFromDate = r.RoomStatusFromDate,
                RoomStatusToDate = r.RoomStatusToDate
            })
            .ToListAsync();
    }

    public async Task<RoomDto> GetByIdAsync(int id)
    {
        var room = await _context.Rooms
            .Include(r => r.RoomType)
            .FirstOrDefaultAsync(r => r.RoomId == id);

        if (room == null)
            throw new NotFoundException(nameof(RoomNo), id);

        return new RoomDto
        {
            RoomId = room.RoomId,
            RoomNumber = room.RoomNumber,
            RoomTypeId = room.RoomTypeId,
            RoomTypeName = room.RoomType.RoomType,
            RoomStatus = room.RoomStatus,
            RoomStatusFromDate = room.RoomStatusFromDate,
            RoomStatusToDate = room.RoomStatusToDate
        };
    }

    public async Task<List<RoomDto>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut)
    {
        // Get all rooms
        var allRooms = await _context.Rooms
            .Include(r => r.RoomType)
            .ToListAsync();

        // Get occupied rooms in date range
        var occupiedRoomIds = await _context.CheckIns
            .Where(c => c.Status == CheckInStatus.Active &&
                        c.CheckInDate < checkOut &&
                        c.CheckOutDate > checkIn)
            .Select(c => c.RoomId)
            .ToListAsync();

        // Get rooms with maintenance/blocked status in date range
        var unavailableRoomIds = allRooms
            .Where(r => r.RoomStatus != RoomStatus.Available ||
                        (r.RoomStatusFromDate.HasValue && r.RoomStatusToDate.HasValue &&
                         r.RoomStatusFromDate < checkOut &&
                         r.RoomStatusToDate > checkIn))
            .Select(r => r.RoomId)
            .ToList();

        // Filter available rooms
        var availableRooms = allRooms
            .Where(r => !occupiedRoomIds.Contains(r.RoomId) &&
                        !unavailableRoomIds.Contains(r.RoomId))
            .Select(r => new RoomDto
            {
                RoomId = r.RoomId,
                RoomNumber = r.RoomNumber,
                RoomTypeId = r.RoomTypeId,
                RoomTypeName = r.RoomType.RoomType,
                RoomStatus = r.RoomStatus,
                RoomStatusFromDate = r.RoomStatusFromDate,
                RoomStatusToDate = r.RoomStatusToDate
            })
            .ToList();

        return availableRooms;
    }

    public async Task<RoomDto> CreateAsync(CreateRoomDto dto)
    {
        // Check if room number already exists
        if (await _context.Rooms.AnyAsync(r => r.RoomNumber == dto.RoomNumber))
            throw new BusinessRuleException($"Room number '{dto.RoomNumber}' already exists.");

        // Verify room type exists
        if (!await _context.RoomTypes.AnyAsync(rt => rt.RoomTypeId == dto.RoomTypeId))
            throw new NotFoundException(nameof(MRoomType), dto.RoomTypeId);

        var room = new RoomNo
        {
            RoomNumber = dto.RoomNumber,
            RoomTypeId = dto.RoomTypeId,
            RoomStatus = dto.RoomStatus,
            RoomStatusFromDate = dto.RoomStatusFromDate,
            RoomStatusToDate = dto.RoomStatusToDate
        };

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created room {RoomNumber} with ID {Id}", room.RoomNumber, room.RoomId);

        return await GetByIdAsync(room.RoomId);
    }

    public async Task<RoomDto> UpdateStatusAsync(int id, UpdateRoomStatusDto dto)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
            throw new NotFoundException(nameof(RoomNo), id);

        room.RoomStatus = dto.RoomStatus;
        room.RoomStatusFromDate = dto.RoomStatusFromDate;
        room.RoomStatusToDate = dto.RoomStatusToDate;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated room {RoomNumber} status to {Status}", room.RoomNumber, dto.RoomStatus);

        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
            throw new NotFoundException(nameof(RoomNo), id);

        // Check if room has any check-ins
        if (await _context.CheckIns.AnyAsync(c => c.RoomId == id))
            throw new BusinessRuleException("Cannot delete room that has check-in records.");

        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted room {RoomNumber}", room.RoomNumber);
    }

    public async Task<int> GetRoomIdByNumberAsync(string roomNumber)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomNumber == roomNumber);
        if (room == null)
            throw new NotFoundException($"Room with number '{roomNumber}' not found.");

        return room.RoomId;
    }
}
