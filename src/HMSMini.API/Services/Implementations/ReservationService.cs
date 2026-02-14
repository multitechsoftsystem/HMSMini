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
    private readonly ITariffService _tariffService;
    private readonly ILogger<ReservationService> _logger;

    public ReservationService(
        ApplicationDbContext context,
        IRoomService roomService,
        ITariffService tariffService,
        ILogger<ReservationService> logger)
    {
        _context = context;
        _roomService = roomService;
        _tariffService = tariffService;
        _logger = logger;
    }

    public async Task<List<ReservationDto>> GetAllAsync()
    {
        var reservations = await _context.Reservations
            .Include(r => r.RoomType)
            .Include(r => r.Room)
            .ThenInclude(room => room!.RoomType)
            .Include(r => r.Company)
            .Include(r => r.BusinessSource)
            .Include(r => r.MealPlan)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return reservations.Select(MapToDto).ToList();
    }

    public async Task<ReservationDto> GetByIdAsync(int id)
    {
        var reservation = await _context.Reservations
            .Include(r => r.RoomType)
            .Include(r => r.Room)
            .ThenInclude(room => room!.RoomType)
            .Include(r => r.Company)
            .Include(r => r.BusinessSource)
            .Include(r => r.MealPlan)
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
            .Include(r => r.RoomType)
            .Include(r => r.Room)
            .ThenInclude(room => room!.RoomType)
            .Include(r => r.Company)
            .Include(r => r.BusinessSource)
            .Include(r => r.MealPlan)
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
            .Include(r => r.RoomType)
            .Include(r => r.Room)
            .ThenInclude(room => room!.RoomType)
            .Include(r => r.Company)
            .Include(r => r.BusinessSource)
            .Include(r => r.MealPlan)
            .Where(r => r.Status == status)
            .OrderBy(r => r.CheckInDate)
            .ToListAsync();

        return reservations.Select(MapToDto).ToList();
    }

    public async Task<List<ReservationDto>> GetUpcomingReservationsAsync()
    {
        var today = DateTime.Today;
        var reservations = await _context.Reservations
            .Include(r => r.RoomType)
            .Include(r => r.Room)
            .ThenInclude(room => room!.RoomType)
            .Include(r => r.Company)
            .Include(r => r.BusinessSource)
            .Include(r => r.MealPlan)
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

        // Get room type
        var roomType = await _context.RoomTypes
            .FirstOrDefaultAsync(rt => rt.RoomType == dto.RoomType);

        if (roomType == null)
        {
            throw new NotFoundException($"Room type '{dto.RoomType}' was not found.");
        }

        // Calculate estimated amount if company is provided
        decimal? estimatedAmount = null;
        if (dto.CompanyId.HasValue)
        {
            try
            {
                var tariffCalc = await _tariffService.CalculateTariffAsync(
                    roomType.RoomTypeId,
                    dto.NumberOfGuests,
                    dto.CheckInDate,
                    dto.CheckOutDate,
                    dto.CompanyId,
                    dto.MealPlanId);

                estimatedAmount = tariffCalc.MealPlanId.HasValue ? tariffCalc.TotalAmountWithMealPlan : tariffCalc.TotalAmount;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to calculate tariff for reservation, proceeding without estimated amount");
            }
        }

        // Generate unique reservation number
        var reservationNumber = await GenerateReservationNumberAsync();

        var reservation = new Reservation
        {
            ReservationNumber = reservationNumber,
            RoomTypeId = roomType.RoomTypeId,
            RoomId = null, // Room will be assigned at check-in
            CheckInDate = dto.CheckInDate,
            CheckOutDate = dto.CheckOutDate,
            NumberOfGuests = dto.NumberOfGuests,
            GuestName = dto.GuestName,
            GuestEmail = dto.GuestEmail,
            GuestMobile = dto.GuestMobile,
            SpecialRequests = dto.SpecialRequests,
            Status = ReservationStatus.Pending,
            CompanyId = dto.CompanyId,
            BusinessSourceId = dto.BusinessSourceId,
            MealPlanId = dto.MealPlanId,
            EstimatedAmount = estimatedAmount
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Reservation created: {ReservationNumber} for room type {RoomType}",
            reservationNumber, dto.RoomType);

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

    public async Task<ReservationDto> AssignRoomAsync(int id, AssignRoomDto dto)
    {
        var reservation = await _context.Reservations
            .Include(r => r.RoomType)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reservation == null)
        {
            throw new NotFoundException($"Reservation with key '{id}' was not found.");
        }

        // Cannot assign room to cancelled or checked-in reservations
        if (reservation.Status == ReservationStatus.Cancelled)
        {
            throw new BusinessRuleException("Cannot assign room to a cancelled reservation.");
        }

        if (reservation.Status == ReservationStatus.CheckedIn)
        {
            throw new BusinessRuleException("Cannot assign room to a reservation that has been checked in.");
        }

        // Verify the room exists and matches the reservation's room type
        var room = await _context.Rooms
            .Include(r => r.RoomType)
            .FirstOrDefaultAsync(r => r.RoomId == dto.RoomId);

        if (room == null)
        {
            throw new NotFoundException($"Room with ID '{dto.RoomId}' was not found.");
        }

        if (room.RoomTypeId != reservation.RoomTypeId)
        {
            throw new BusinessRuleException($"Room {room.RoomNumber} is type '{room.RoomType.RoomType}' but reservation requires '{reservation.RoomType.RoomType}'.");
        }

        // Check if the room is available for the reservation dates
        var isAvailable = await IsRoomAvailableAsync(dto.RoomId, reservation.CheckInDate, reservation.CheckOutDate, reservation.Id);
        if (!isAvailable)
        {
            throw new BusinessRuleException($"Room {room.RoomNumber} is not available for the selected dates.");
        }

        reservation.RoomId = dto.RoomId;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Room {RoomNumber} assigned to reservation {ReservationNumber}",
            room.RoomNumber, reservation.ReservationNumber);

        return await GetByIdAsync(id);
    }

    public async Task<ReservationDto> ExtendStayAsync(int id, ExtendStayDto dto)
    {
        var reservation = await _context.Reservations
            .Include(r => r.RoomType)
            .Include(r => r.Room)
            .Include(r => r.Company)
            .Include(r => r.BusinessSource)
            .Include(r => r.MealPlan)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reservation == null)
            throw new NotFoundException($"Reservation with key '{id}' was not found.");

        if (reservation.Status == ReservationStatus.CheckedIn)
            throw new BusinessRuleException("Cannot extend a reservation that has already been checked in. Please extend the check-in instead.");

        if (reservation.Status == ReservationStatus.Cancelled)
            throw new BusinessRuleException("Cannot extend a cancelled reservation.");

        if (dto.NewCheckOutDate <= reservation.CheckOutDate)
            throw new BusinessRuleException("New checkout date must be after the current checkout date.");

        if (dto.NewCheckOutDate <= DateTime.Today)
            throw new BusinessRuleException("New checkout date must be in the future.");

        // Update checkout date
        var oldCheckOutDate = reservation.CheckOutDate;
        reservation.CheckOutDate = dto.NewCheckOutDate;

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Extended reservation {ReservationNumber} from {OldDate} to {NewDate}",
            reservation.ReservationNumber, oldCheckOutDate, dto.NewCheckOutDate);

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

    private async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkInDate, DateTime checkOutDate, int? excludeReservationId = null)
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

        // Check for existing reservations (excluding the current one if specified)
        var reservationsQuery = _context.Reservations
            .Where(r => r.RoomId == roomId &&
                       (r.Status == ReservationStatus.Confirmed || r.Status == ReservationStatus.Pending) &&
                       r.CheckInDate < checkOutDate &&
                       r.CheckOutDate > checkInDate);

        if (excludeReservationId.HasValue)
        {
            reservationsQuery = reservationsQuery.Where(r => r.Id != excludeReservationId.Value);
        }

        var hasConflictingReservation = await reservationsQuery.AnyAsync();

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
            RoomNumber = reservation.Room?.RoomNumber,
            RoomTypeName = reservation.RoomType.RoomType,
            CheckInDate = reservation.CheckInDate,
            CheckOutDate = reservation.CheckOutDate,
            NumberOfGuests = reservation.NumberOfGuests,
            GuestName = reservation.GuestName,
            GuestEmail = reservation.GuestEmail,
            GuestMobile = reservation.GuestMobile,
            SpecialRequests = reservation.SpecialRequests,
            Status = reservation.Status,
            CheckInId = reservation.CheckInId,
            CompanyId = reservation.CompanyId,
            CompanyName = reservation.Company?.CompanyName,
            CompanyGSTNumber = reservation.Company?.GSTNumber,
            BusinessSourceId = reservation.BusinessSourceId,
            BusinessSourceName = reservation.BusinessSource?.SourceName,
            MealPlanId = reservation.MealPlanId,
            MealPlanName = reservation.MealPlan?.PlanName,
            EstimatedAmount = reservation.EstimatedAmount,
            CreatedAt = reservation.CreatedAt
        };
    }
}
