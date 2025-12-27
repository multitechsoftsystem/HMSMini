using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.Reservation;
using HMSMini.API.Models.Entities;
using HMSMini.API.Models.Enums;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Services.Implementations;

/// <summary>
/// Service implementation for reservation operations
/// </summary>
public class ReservationService : IReservationService
{
    private readonly ApplicationDbContext _context;
    private readonly IRoomService _roomService;
    private readonly ILogger<ReservationService> _logger;

    public ReservationService(
        ApplicationDbContext context,
        IRoomService roomService,
        ILogger<ReservationService> logger)
    {
        _context = context;
        _roomService = roomService;
        _logger = logger;
    }

    public async Task<List<ReservationDto>> GetAllAsync()
    {
        var reservations = await _context.Reservations
            .Include(r => r.Room)
            .ThenInclude(room => room.RoomType)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return reservations.Select(MapToDto).ToList();
    }

    public async Task<ReservationDto> GetByIdAsync(int id)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Room)
            .ThenInclude(room => room.RoomType)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reservation == null)
        {
            throw new NotFoundException($"Reservation with key '{id}' was not found.");
        }

        return MapToDto(reservation);
    }

    public async Task<ReservationDto> GetByReservationNumberAsync(string reservationNumber)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Room)
            .ThenInclude(room => room.RoomType)
            .FirstOrDefaultAsync(r => r.ReservationNumber == reservationNumber);

        if (reservation == null)
        {
            throw new NotFoundException($"Reservation with number '{reservationNumber}' was not found.");
        }

        return MapToDto(reservation);
    }

    public async Task<List<ReservationDto>> GetByStatusAsync(ReservationStatus status)
    {
        var reservations = await _context.Reservations
            .Include(r => r.Room)
            .ThenInclude(room => room.RoomType)
            .Where(r => r.Status == status)
            .OrderBy(r => r.CheckInDate)
            .ToListAsync();

        return reservations.Select(MapToDto).ToList();
    }

    public async Task<List<ReservationDto>> GetUpcomingReservationsAsync()
    {
        var today = DateTime.Today;
        var reservations = await _context.Reservations
            .Include(r => r.Room)
            .ThenInclude(room => room.RoomType)
            .Where(r => r.CheckInDate >= today &&
                       (r.Status == ReservationStatus.Confirmed || r.Status == ReservationStatus.Pending))
            .OrderBy(r => r.CheckInDate)
            .ToListAsync();

        return reservations.Select(MapToDto).ToList();
    }

    public async Task<ReservationDto> CreateAsync(CreateReservationDto dto)
    {
        // Validate dates
        if (dto.CheckOutDate <= dto.CheckInDate)
        {
            throw new BusinessRuleException("Check-out date must be after check-in date.");
        }

        if (dto.CheckInDate < DateTime.Today)
        {
            throw new BusinessRuleException("Check-in date cannot be in the past.");
        }

        // Validate number of guests
        if (dto.NumberOfGuests < 1 || dto.NumberOfGuests > 3)
        {
            throw new BusinessRuleException("Number of guests must be between 1 and 3.");
        }

        // Get room
        var roomId = await _roomService.GetRoomIdByNumberAsync(dto.RoomNumber);

        // Check if room is available for the specified dates
        var isAvailable = await IsRoomAvailableAsync(roomId, dto.CheckInDate, dto.CheckOutDate);
        if (!isAvailable)
        {
            throw new BusinessRuleException("Room is not available for the selected dates.");
        }

        // Generate unique reservation number
        var reservationNumber = await GenerateReservationNumberAsync();

        var reservation = new Reservation
        {
            ReservationNumber = reservationNumber,
            RoomId = roomId,
            CheckInDate = dto.CheckInDate,
            CheckOutDate = dto.CheckOutDate,
            NumberOfGuests = dto.NumberOfGuests,
            GuestName = dto.GuestName,
            GuestEmail = dto.GuestEmail,
            GuestMobile = dto.GuestMobile,
            SpecialRequests = dto.SpecialRequests,
            Status = ReservationStatus.Pending
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Reservation created: {ReservationNumber} for room {RoomNumber}",
            reservationNumber, dto.RoomNumber);

        return await GetByIdAsync(reservation.Id);
    }

    public async Task<ReservationDto> UpdateAsync(int id, UpdateReservationDto dto)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null)
        {
            throw new NotFoundException($"Reservation with key '{id}' was not found.");
        }

        // Cannot update checked-in or cancelled reservations
        if (reservation.Status == ReservationStatus.CheckedIn)
        {
            throw new BusinessRuleException("Cannot update a reservation that has been checked in.");
        }

        if (reservation.Status == ReservationStatus.Cancelled)
        {
            throw new BusinessRuleException("Cannot update a cancelled reservation.");
        }

        // Update fields if provided
        if (dto.CheckInDate.HasValue)
        {
            reservation.CheckInDate = dto.CheckInDate.Value;
        }

        if (dto.CheckOutDate.HasValue)
        {
            reservation.CheckOutDate = dto.CheckOutDate.Value;
        }

        // Validate dates
        if (reservation.CheckOutDate <= reservation.CheckInDate)
        {
            throw new BusinessRuleException("Check-out date must be after check-in date.");
        }

        if (dto.NumberOfGuests.HasValue)
        {
            if (dto.NumberOfGuests.Value < 1 || dto.NumberOfGuests.Value > 3)
            {
                throw new BusinessRuleException("Number of guests must be between 1 and 3.");
            }
            reservation.NumberOfGuests = dto.NumberOfGuests.Value;
        }

        if (!string.IsNullOrWhiteSpace(dto.GuestName))
        {
            reservation.GuestName = dto.GuestName;
        }

        if (dto.GuestEmail != null)
        {
            reservation.GuestEmail = dto.GuestEmail;
        }

        if (!string.IsNullOrWhiteSpace(dto.GuestMobile))
        {
            reservation.GuestMobile = dto.GuestMobile;
        }

        if (dto.SpecialRequests != null)
        {
            reservation.SpecialRequests = dto.SpecialRequests;
        }

        if (dto.Status.HasValue)
        {
            reservation.Status = dto.Status.Value;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Reservation updated: {ReservationNumber}", reservation.ReservationNumber);

        return await GetByIdAsync(id);
    }

    public async Task<ReservationDto> ConfirmReservationAsync(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null)
        {
            throw new NotFoundException($"Reservation with key '{id}' was not found.");
        }

        if (reservation.Status != ReservationStatus.Pending)
        {
            throw new BusinessRuleException("Only pending reservations can be confirmed.");
        }

        reservation.Status = ReservationStatus.Confirmed;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Reservation confirmed: {ReservationNumber}", reservation.ReservationNumber);

        return await GetByIdAsync(id);
    }

    public async Task CancelReservationAsync(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null)
        {
            throw new NotFoundException($"Reservation with key '{id}' was not found.");
        }

        if (reservation.Status == ReservationStatus.CheckedIn)
        {
            throw new BusinessRuleException("Cannot cancel a reservation that has been checked in.");
        }

        if (reservation.Status == ReservationStatus.Cancelled)
        {
            throw new BusinessRuleException("Reservation is already cancelled.");
        }

        reservation.Status = ReservationStatus.Cancelled;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Reservation cancelled: {ReservationNumber}", reservation.ReservationNumber);
    }

    public async Task MarkAsCheckedInAsync(int reservationId, int checkInId)
    {
        var reservation = await _context.Reservations.FindAsync(reservationId);
        if (reservation == null)
        {
            throw new NotFoundException($"Reservation with key '{reservationId}' was not found.");
        }

        reservation.Status = ReservationStatus.CheckedIn;
        reservation.CheckInId = checkInId;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Reservation marked as checked in: {ReservationNumber}", reservation.ReservationNumber);
    }

    public async Task DeleteAsync(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null)
        {
            throw new NotFoundException($"Reservation with key '{id}' was not found.");
        }

        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Reservation deleted: {ReservationNumber}", reservation.ReservationNumber);
    }

    private async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkInDate, DateTime checkOutDate)
    {
        // Check for existing active check-ins
        var hasActiveCheckIn = await _context.CheckIns
            .AnyAsync(c => c.RoomId == roomId &&
                          c.Status == CheckInStatus.Active &&
                          c.CheckInDate < checkOutDate &&
                          c.CheckOutDate > checkInDate);

        if (hasActiveCheckIn)
        {
            return false;
        }

        // Check for existing reservations
        var hasConflictingReservation = await _context.Reservations
            .AnyAsync(r => r.RoomId == roomId &&
                          (r.Status == ReservationStatus.Confirmed || r.Status == ReservationStatus.Pending) &&
                          r.CheckInDate < checkOutDate &&
                          r.CheckOutDate > checkInDate);

        if (hasConflictingReservation)
        {
            return false;
        }

        // Check room status
        var room = await _context.Rooms.FindAsync(roomId);
        if (room == null || room.RoomStatus != RoomStatus.Available)
        {
            // Check if room will be available during the reservation period
            if (room!.RoomStatusFromDate.HasValue && room.RoomStatusToDate.HasValue)
            {
                var statusOverlaps = room.RoomStatusFromDate.Value < checkOutDate &&
                                    room.RoomStatusToDate.Value > checkInDate;
                if (statusOverlaps)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private async Task<string> GenerateReservationNumberAsync()
    {
        var today = DateTime.Today;
        var prefix = $"RES{today:yyyyMMdd}";

        var lastReservation = await _context.Reservations
            .Where(r => r.ReservationNumber.StartsWith(prefix))
            .OrderByDescending(r => r.ReservationNumber)
            .FirstOrDefaultAsync();

        int sequence = 1;
        if (lastReservation != null)
        {
            var lastSequence = lastReservation.ReservationNumber.Substring(prefix.Length);
            if (int.TryParse(lastSequence, out var parsedSequence))
            {
                sequence = parsedSequence + 1;
            }
        }

        return $"{prefix}{sequence:D4}";
    }

    private static ReservationDto MapToDto(Reservation reservation)
    {
        return new ReservationDto
        {
            Id = reservation.Id,
            ReservationNumber = reservation.ReservationNumber,
            RoomNumber = reservation.Room.RoomNumber,
            RoomTypeName = reservation.Room.RoomType.RoomType,
            CheckInDate = reservation.CheckInDate,
            CheckOutDate = reservation.CheckOutDate,
            NumberOfGuests = reservation.NumberOfGuests,
            GuestName = reservation.GuestName,
            GuestEmail = reservation.GuestEmail,
            GuestMobile = reservation.GuestMobile,
            SpecialRequests = reservation.SpecialRequests,
            Status = reservation.Status,
            CheckInId = reservation.CheckInId,
            CreatedAt = reservation.CreatedAt
        };
    }
}
