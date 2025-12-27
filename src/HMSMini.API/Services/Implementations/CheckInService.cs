using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.CheckIn;
using HMSMini.API.Models.DTOs.Guest;
using HMSMini.API.Models.Entities;
using HMSMini.API.Models.Enums;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Services.Implementations;

public class CheckInService : ICheckInService
{
    private readonly ApplicationDbContext _context;
    private readonly IRoomService _roomService;
    private readonly ILogger<CheckInService> _logger;

    public CheckInService(
        ApplicationDbContext context,
        IRoomService roomService,
        ILogger<CheckInService> logger)
    {
        _context = context;
        _roomService = roomService;
        _logger = logger;
    }

    public async Task<CheckInWithGuestsDto> GetByIdAsync(int id)
    {
        var checkIn = await _context.CheckIns
            .Include(c => c.Room)
            .ThenInclude(r => r.RoomType)
            .Include(c => c.Guests)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (checkIn == null)
            throw new NotFoundException(nameof(CheckIn), id);

        return new CheckInWithGuestsDto
        {
            Id = checkIn.Id,
            RoomId = checkIn.RoomId,
            RoomNumber = checkIn.Room.RoomNumber,
            RoomTypeName = checkIn.Room.RoomType.RoomType,
            CheckInDate = checkIn.CheckInDate,
            CheckOutDate = checkIn.CheckOutDate,
            ActualCheckOutDate = checkIn.ActualCheckOutDate,
            Pax = checkIn.Pax,
            Status = checkIn.Status,
            Guests = checkIn.Guests.Select(g => new GuestDto
            {
                Id = g.Id,
                CheckInId = g.CheckInId,
                GuestNumber = g.GuestNumber,
                GuestName = g.GuestName,
                Address = g.Address,
                City = g.City,
                State = g.State,
                Country = g.Country,
                MobileNo = g.MobileNo,
                Photo1Path = g.Photo1Path,
                Photo2Path = g.Photo2Path
            }).ToList(),
            CreatedAt = checkIn.CreatedAt,
            UpdatedAt = checkIn.UpdatedAt
        };
    }

    public async Task<List<CheckInDto>> GetAllAsync()
    {
        return await _context.CheckIns
            .Include(c => c.Room)
            .Select(c => new CheckInDto
            {
                Id = c.Id,
                RoomId = c.RoomId,
                RoomNumber = c.Room.RoomNumber,
                CheckInDate = c.CheckInDate,
                CheckOutDate = c.CheckOutDate,
                ActualCheckOutDate = c.ActualCheckOutDate,
                Pax = c.Pax,
                Status = c.Status,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .OrderByDescending(c => c.CheckInDate)
            .ToListAsync();
    }

    public async Task<List<CheckInDto>> GetActiveCheckInsAsync()
    {
        return await _context.CheckIns
            .Include(c => c.Room)
            .Where(c => c.Status == CheckInStatus.Active)
            .Select(c => new CheckInDto
            {
                Id = c.Id,
                RoomId = c.RoomId,
                RoomNumber = c.Room.RoomNumber,
                CheckInDate = c.CheckInDate,
                CheckOutDate = c.CheckOutDate,
                ActualCheckOutDate = c.ActualCheckOutDate,
                Pax = c.Pax,
                Status = c.Status,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .OrderByDescending(c => c.CheckInDate)
            .ToListAsync();
    }

    public async Task<CheckInWithGuestsDto> CreateCheckInAsync(CreateCheckInDto dto)
    {
        // Validate dates
        if (dto.CheckOutDate <= dto.CheckInDate)
            throw new BusinessRuleException("Check-out date must be after check-in date.");

        // Validate guest count
        if (dto.Guests.Count < 1 || dto.Guests.Count > 3)
            throw new BusinessRuleException("Number of guests must be between 1 and 3.");

        // Get room ID
        var roomId = await _roomService.GetRoomIdByNumberAsync(dto.RoomNumber);

        // Check room availability
        var availableRooms = await _roomService.GetAvailableRoomsAsync(dto.CheckInDate, dto.CheckOutDate);
        if (!availableRooms.Any(r => r.RoomId == roomId))
            throw new BusinessRuleException($"Room {dto.RoomNumber} is not available for the selected dates.");

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Create check-in
            var checkIn = new CheckIn
            {
                RoomId = roomId,
                CheckInDate = dto.CheckInDate,
                CheckOutDate = dto.CheckOutDate,
                Pax = dto.Guests.Count,
                Status = CheckInStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            _context.CheckIns.Add(checkIn);
            await _context.SaveChangesAsync();

            // Create guests
            for (int i = 0; i < dto.Guests.Count; i++)
            {
                var guestDto = dto.Guests[i];
                var guest = new Guest
                {
                    CheckInId = checkIn.Id,
                    GuestNumber = i + 1,
                    GuestName = guestDto.GuestName,
                    Address = guestDto.Address,
                    City = guestDto.City,
                    State = guestDto.State,
                    Country = guestDto.Country,
                    MobileNo = guestDto.MobileNo
                };
                _context.Guests.Add(guest);
            }

            // Update room status to Occupied
            var room = await _context.Rooms.FindAsync(roomId);
            if (room != null)
            {
                room.RoomStatus = RoomStatus.Occupied;
                room.RoomStatusFromDate = dto.CheckInDate;
                room.RoomStatusToDate = dto.CheckOutDate;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Created check-in ID {Id} for room {RoomNumber}", checkIn.Id, dto.RoomNumber);

            return await GetByIdAsync(checkIn.Id);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task CheckOutAsync(int id)
    {
        var checkIn = await _context.CheckIns
            .Include(c => c.Room)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (checkIn == null)
            throw new NotFoundException(nameof(CheckIn), id);

        if (checkIn.Status != CheckInStatus.Active)
            throw new BusinessRuleException("Only active check-ins can be checked out.");

        checkIn.Status = CheckInStatus.CheckedOut;
        checkIn.ActualCheckOutDate = DateTime.UtcNow;
        checkIn.UpdatedAt = DateTime.UtcNow;

        // Update room status to Dirty (needs cleaning after checkout)
        if (checkIn.Room != null)
        {
            checkIn.Room.RoomStatus = RoomStatus.Dirty;
            checkIn.Room.RoomStatusFromDate = null;
            checkIn.Room.RoomStatusToDate = null;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Checked out check-in ID {Id}", id);
    }

    public async Task DeleteAsync(int id)
    {
        var checkIn = await _context.CheckIns
            .Include(c => c.Room)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (checkIn == null)
            throw new NotFoundException(nameof(CheckIn), id);

        // Free up the room if it was occupied
        if (checkIn.Status == CheckInStatus.Active && checkIn.Room != null)
        {
            checkIn.Room.RoomStatus = RoomStatus.Available;
            checkIn.Room.RoomStatusFromDate = null;
            checkIn.Room.RoomStatusToDate = null;
        }

        _context.CheckIns.Remove(checkIn);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted check-in ID {Id}", id);
    }
}
