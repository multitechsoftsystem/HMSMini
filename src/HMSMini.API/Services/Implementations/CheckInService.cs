using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.CheckIn;
using HMSMini.API.Models.DTOs.Guest;
using HMSMini.API.Models.Entities;
using HMSMini.API.Models.Enums;
using HMSMini.API.Services.Interfaces;
using System.Text.Json;

namespace HMSMini.API.Services.Implementations;

public class CheckInService : ICheckInService
{
    private readonly ApplicationDbContext _context;
    private readonly IRoomService _roomService;
    private readonly ITariffService _tariffService;
    private readonly ITaxService _taxService;
    private readonly ISystemSettingsService _systemSettingsService;
    private readonly ILogger<CheckInService> _logger;

    public CheckInService(
        ApplicationDbContext context,
        IRoomService roomService,
        ITariffService tariffService,
        ITaxService taxService,
        ISystemSettingsService systemSettingsService,
        ILogger<CheckInService> logger)
    {
        _context = context;
        _roomService = roomService;
        _tariffService = tariffService;
        _taxService = taxService;
        _systemSettingsService = systemSettingsService;
        _logger = logger;
    }

    public async Task<CheckInWithGuestsDto> GetByIdAsync(int id)
    {
        var checkIn = await _context.CheckIns
            .Include(c => c.Room)
            .ThenInclude(r => r.RoomType)
            .Include(c => c.Guests)
            .Include(c => c.Company)
            .Include(c => c.BusinessSource)
            .Include(c => c.MealPlan)
            .Include(c => c.GuestType)
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
            ActualCheckInDate = checkIn.ActualCheckInDate,
            ActualCheckOutDate = checkIn.ActualCheckOutDate,
            RegistrationNo = checkIn.RegistrationNo,
            Pax = checkIn.Pax,
            Status = checkIn.Status,
            Remarks = checkIn.Remarks,
            TaxType = checkIn.TaxType,
            CompanyId = checkIn.CompanyId,
            CompanyName = checkIn.Company?.CompanyName,
            CompanyGSTNumber = checkIn.Company?.GSTNumber,
            BusinessSourceId = checkIn.BusinessSourceId,
            BusinessSourceName = checkIn.BusinessSource?.SourceName,
            MealPlanId = checkIn.MealPlanId,
            MealPlanName = checkIn.MealPlan?.PlanName,
            GuestTypeId = checkIn.GuestTypeId,
            GuestTypeName = checkIn.GuestType?.TypeName,
            MealPlanRate = checkIn.MealPlanRate,
            TariffApplied = checkIn.TariffApplied,
            DiscountPercentage = checkIn.DiscountPercentage,
            FinalAmount = checkIn.FinalAmount,
            IsSharedRoom = checkIn.IsSharedRoom,
            SharedGroupId = checkIn.SharedGroupId,
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
                PanOrAadharNo = g.PanOrAadharNo,
                Photo1Path = g.Photo1Path,
                Photo2Path = g.Photo2Path,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt,
                CreatedBy = g.CreatedBy,
                UpdatedBy = g.UpdatedBy
            }).ToList(),
            CreatedAt = checkIn.CreatedAt,
            UpdatedAt = checkIn.UpdatedAt,
            CreatedBy = checkIn.CreatedBy,
            UpdatedBy = checkIn.UpdatedBy
        };
    }

    public async Task<List<CheckInDto>> GetAllAsync()
    {
        return await _context.CheckIns
            .Include(c => c.Room)
            .Include(c => c.Guests)
            .Include(c => c.Company)
            .Include(c => c.BusinessSource)
            .Include(c => c.MealPlan)
            .Include(c => c.GuestType)
            .Select(c => new CheckInDto
            {
                Id = c.Id,
                RoomId = c.RoomId,
                RoomNumber = c.Room.RoomNumber,
                CheckInDate = c.CheckInDate,
                CheckOutDate = c.CheckOutDate,
                ActualCheckInDate = c.ActualCheckInDate,
                ActualCheckOutDate = c.ActualCheckOutDate,
                RegistrationNo = c.RegistrationNo,
                Pax = c.Pax,
                Status = c.Status,
                Remarks = c.Remarks,
                TaxType = c.TaxType,
                CompanyId = c.CompanyId,
                CompanyName = c.Company != null ? c.Company.CompanyName : null,
                CompanyGSTNumber = c.Company != null ? c.Company.GSTNumber : null,
                BusinessSourceId = c.BusinessSourceId,
                BusinessSourceName = c.BusinessSource != null ? c.BusinessSource.SourceName : null,
                MealPlanId = c.MealPlanId,
                MealPlanName = c.MealPlan != null ? c.MealPlan.PlanName : null,
                GuestTypeId = c.GuestTypeId,
                GuestTypeName = c.GuestType != null ? c.GuestType.TypeName : null,
                MealPlanRate = c.MealPlanRate,
                TariffApplied = c.TariffApplied,
                DiscountPercentage = c.DiscountPercentage,
                FinalAmount = c.FinalAmount,
                IsSharedRoom = c.IsSharedRoom,
                SharedGroupId = c.SharedGroupId,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                CreatedBy = c.CreatedBy,
                UpdatedBy = c.UpdatedBy,
                GuestNames = string.Join(", ", c.Guests.OrderBy(g => g.GuestNumber).Select(g => g.GuestName))
            })
            .OrderByDescending(c => c.CheckInDate)
            .ToListAsync();
    }

    public async Task<List<CheckInDto>> GetActiveCheckInsAsync()
    {
        return await _context.CheckIns
            .Include(c => c.Room)
            .Include(c => c.Guests)
            .Include(c => c.Company)
            .Include(c => c.BusinessSource)
            .Include(c => c.MealPlan)
            .Include(c => c.GuestType)
            .Where(c => c.Status == CheckInStatus.Active)
            .Select(c => new CheckInDto
            {
                Id = c.Id,
                RoomId = c.RoomId,
                RoomNumber = c.Room.RoomNumber,
                CheckInDate = c.CheckInDate,
                CheckOutDate = c.CheckOutDate,
                ActualCheckInDate = c.ActualCheckInDate,
                ActualCheckOutDate = c.ActualCheckOutDate,
                RegistrationNo = c.RegistrationNo,
                Pax = c.Pax,
                Status = c.Status,
                Remarks = c.Remarks,
                TaxType = c.TaxType,
                CompanyId = c.CompanyId,
                CompanyName = c.Company != null ? c.Company.CompanyName : null,
                CompanyGSTNumber = c.Company != null ? c.Company.GSTNumber : null,
                BusinessSourceId = c.BusinessSourceId,
                BusinessSourceName = c.BusinessSource != null ? c.BusinessSource.SourceName : null,
                MealPlanId = c.MealPlanId,
                MealPlanName = c.MealPlan != null ? c.MealPlan.PlanName : null,
                GuestTypeId = c.GuestTypeId,
                GuestTypeName = c.GuestType != null ? c.GuestType.TypeName : null,
                MealPlanRate = c.MealPlanRate,
                TariffApplied = c.TariffApplied,
                DiscountPercentage = c.DiscountPercentage,
                FinalAmount = c.FinalAmount,
                IsSharedRoom = c.IsSharedRoom,
                SharedGroupId = c.SharedGroupId,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                CreatedBy = c.CreatedBy,
                UpdatedBy = c.UpdatedBy,
                GuestNames = string.Join(", ", c.Guests.OrderBy(g => g.GuestNumber).Select(g => g.GuestName))
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

        // Calculate tariff for all guests (walk-in, corporate, with/without meal plan)
        decimal? tariffApplied = null;
        decimal discountPercentage = 0;
        decimal? finalAmount = null;
        decimal? mealPlanRate = null;

        var roomForTariff = await _context.Rooms
            .Include(r => r.RoomType)
            .FirstOrDefaultAsync(r => r.RoomId == roomId);

        if (roomForTariff != null)
        {
            try
            {
                var tariffCalc = await _tariffService.CalculateTariffAsync(
                    roomForTariff.RoomTypeId,
                    dto.Guests.Count,
                    dto.CheckInDate,
                    dto.CheckOutDate,
                    dto.CompanyId,
                    dto.MealPlanId);

                tariffApplied = tariffCalc.ApplicableRate;
                discountPercentage = tariffCalc.DiscountPercentage;
                finalAmount = tariffCalc.MealPlanId.HasValue ? tariffCalc.TotalAmountWithMealPlan : tariffCalc.TotalAmount;
                mealPlanRate = tariffCalc.MealPlanId.HasValue ? tariffCalc.MealPlanTotalRate : null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to calculate tariff for check-in, proceeding without pricing");
            }
        }

        // Create tax snapshot at check-in time for historical accuracy
        var taxSnapshot = await _taxService.CreateTaxSlabSnapshotAsync(dto.ActualCheckInDate ?? DateTime.UtcNow);
        var taxSnapshotJson = JsonSerializer.Serialize(taxSnapshot);

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Create check-in
            var checkIn = new CheckIn
            {
                RoomId = roomId,
                CheckInDate = dto.CheckInDate,
                CheckOutDate = dto.CheckOutDate,
                ActualCheckInDate = dto.ActualCheckInDate ?? DateTime.UtcNow,
                RegistrationNo = dto.RegistrationNo,
                Pax = dto.Guests.Count,
                Status = CheckInStatus.Active,
                Remarks = dto.Remarks,
                CompanyId = dto.CompanyId,
                BusinessSourceId = dto.BusinessSourceId,
                MealPlanId = dto.MealPlanId,
                GuestTypeId = dto.GuestTypeId,
                MealPlanRate = mealPlanRate,
                TariffApplied = tariffApplied,
                DiscountPercentage = discountPercentage,
                FinalAmount = finalAmount,
                TaxType = dto.TaxType,
                TaxSlabSnapshotJson = taxSnapshotJson,
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
                    MobileNo = guestDto.MobileNo,
                    PanOrAadharNo = guestDto.PanOrAadharNo,
                    CreatedAt = DateTime.UtcNow
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

    public async Task<CheckInDto> ExtendStayAsync(int id, ExtendStayDto dto)
    {
        var checkIn = await _context.CheckIns
            .Include(c => c.Room)
            .ThenInclude(r => r.RoomType)
            .Include(c => c.Company)
            .Include(c => c.BusinessSource)
            .Include(c => c.MealPlan)
            .Include(c => c.GuestType)
            .Include(c => c.Guests)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (checkIn == null)
            throw new NotFoundException(nameof(CheckIn), id);

        if (checkIn.Status != CheckInStatus.Active)
            throw new BusinessRuleException("Only active check-ins can be extended.");

        if (dto.NewCheckOutDate <= checkIn.CheckOutDate)
            throw new BusinessRuleException("New checkout date must be after the current checkout date.");

        if (dto.NewCheckOutDate <= DateTime.Today)
            throw new BusinessRuleException("New checkout date must be in the future.");

        // Update checkout date
        var oldCheckOutDate = checkIn.CheckOutDate;
        checkIn.CheckOutDate = dto.NewCheckOutDate;
        checkIn.UpdatedAt = DateTime.UtcNow;

        // Update room status dates - take max of ALL active check-ins' checkout dates
        if (checkIn.Room != null && checkIn.Room.RoomStatus == RoomStatus.Occupied)
        {
            var otherMaxCheckOut = await _context.CheckIns
                .Where(c => c.RoomId == checkIn.RoomId && c.Status == CheckInStatus.Active && c.Id != checkIn.Id)
                .Select(c => (DateTime?)c.CheckOutDate)
                .MaxAsync();
            // Compare with the in-memory updated value for the current check-in
            var maxCheckOutDate = otherMaxCheckOut.HasValue && otherMaxCheckOut.Value > dto.NewCheckOutDate
                ? otherMaxCheckOut.Value
                : dto.NewCheckOutDate;
            checkIn.Room.RoomStatusToDate = maxCheckOutDate;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Extended stay for check-in {CheckInId} from {OldDate} to {NewDate}",
            id, oldCheckOutDate, dto.NewCheckOutDate);

        // Return updated check-in
        return new CheckInDto
        {
            Id = checkIn.Id,
            RoomId = checkIn.RoomId,
            RoomNumber = checkIn.Room.RoomNumber,
            CheckInDate = checkIn.CheckInDate,
            CheckOutDate = checkIn.CheckOutDate,
            ActualCheckInDate = checkIn.ActualCheckInDate,
            ActualCheckOutDate = checkIn.ActualCheckOutDate,
            RegistrationNo = checkIn.RegistrationNo,
            Pax = checkIn.Pax,
            Status = checkIn.Status,
            Remarks = checkIn.Remarks,
            TaxType = checkIn.TaxType,
            CompanyId = checkIn.CompanyId,
            CompanyName = checkIn.Company?.CompanyName,
            CompanyGSTNumber = checkIn.Company?.GSTNumber,
            BusinessSourceId = checkIn.BusinessSourceId,
            BusinessSourceName = checkIn.BusinessSource?.SourceName,
            MealPlanId = checkIn.MealPlanId,
            MealPlanName = checkIn.MealPlan?.PlanName,
            GuestTypeId = checkIn.GuestTypeId,
            GuestTypeName = checkIn.GuestType?.TypeName,
            MealPlanRate = checkIn.MealPlanRate,
            TariffApplied = checkIn.TariffApplied,
            DiscountPercentage = checkIn.DiscountPercentage,
            FinalAmount = checkIn.FinalAmount,
            IsSharedRoom = checkIn.IsSharedRoom,
            SharedGroupId = checkIn.SharedGroupId,
            CreatedAt = checkIn.CreatedAt,
            UpdatedAt = checkIn.UpdatedAt,
            CreatedBy = checkIn.CreatedBy,
            UpdatedBy = checkIn.UpdatedBy,
            GuestNames = string.Join(", ", checkIn.Guests.OrderBy(g => g.GuestNumber).Select(g => g.GuestName))
        };
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

        // Check for other active check-ins on the same room
        var otherActiveCheckIns = await _context.CheckIns
            .Where(c => c.RoomId == checkIn.RoomId && c.Status == CheckInStatus.Active && c.Id != id)
            .ToListAsync();

        if (otherActiveCheckIns.Any())
        {
            // Room stays Occupied, update date range to cover remaining check-ins
            if (checkIn.Room != null)
            {
                checkIn.Room.RoomStatusFromDate = otherActiveCheckIns.Min(c => c.CheckInDate);
                checkIn.Room.RoomStatusToDate = otherActiveCheckIns.Max(c => c.CheckOutDate);
            }

            // If only 1 remains, un-share it
            if (otherActiveCheckIns.Count == 1)
            {
                otherActiveCheckIns[0].IsSharedRoom = false;
                otherActiveCheckIns[0].SharedGroupId = null;
            }
        }
        else
        {
            // No other active check-ins: set room to Dirty
            if (checkIn.Room != null)
            {
                checkIn.Room.RoomStatus = RoomStatus.Dirty;
                checkIn.Room.RoomStatusFromDate = null;
                checkIn.Room.RoomStatusToDate = null;
            }
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Checked out check-in ID {Id}", id);
    }

    public async Task<CheckInDto?> UpdateCheckInAsync(int id, UpdateCheckInDto dto)
    {
        var checkIn = await _context.CheckIns
            .Include(c => c.Room)
            .Include(c => c.Company)
            .Include(c => c.BusinessSource)
            .Include(c => c.MealPlan)
            .Include(c => c.GuestType)
            .FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null);

        if (checkIn == null)
            return null;

        // Only allow updates to active check-ins
        if (checkIn.Status != CheckInStatus.Active)
            throw new InvalidOperationException("Cannot update a checked-out or cancelled check-in");

        // Validate and update checkout date if provided
        if (dto.CheckOutDate.HasValue)
        {
            // Validate checkout date is after check-in date
            if (dto.CheckOutDate.Value <= checkIn.CheckInDate)
                throw new BusinessRuleException("Checkout date must be after check-in date.");

            // Get working date for validation
            var workingDate = await _systemSettingsService.GetWorkingDateAsync();
            if (dto.CheckOutDate.Value < workingDate)
                throw new BusinessRuleException(
                    $"Cannot set checkout date before working date ({workingDate:yyyy-MM-dd}). " +
                    "Past dates are already closed.");

            _logger.LogInformation(
                "Checkout date modified for CheckIn {CheckInId} from {OldDate} to {NewDate}",
                id,
                checkIn.CheckOutDate,
                dto.CheckOutDate.Value);

            checkIn.CheckOutDate = dto.CheckOutDate.Value;
        }

        // Validate Company exists if provided
        if (dto.CompanyId.HasValue)
        {
            var companyExists = await _context.Companies
                .AnyAsync(c => c.Id == dto.CompanyId.Value &&
                              c.IsActive &&
                              c.DeletedAt == null);
            if (!companyExists)
                throw new ArgumentException("Invalid company ID");
        }

        // Validate BusinessSource exists if provided
        if (dto.BusinessSourceId.HasValue)
        {
            var businessSourceExists = await _context.BusinessSources
                .AnyAsync(bs => bs.BusinessSourceId == dto.BusinessSourceId.Value &&
                               bs.IsActive &&
                               bs.DeletedAt == null);
            if (!businessSourceExists)
                throw new ArgumentException("Invalid business source ID");
        }

        // Validate MealPlan exists and recalculate meal plan rate if changed
        decimal? newMealPlanRate = checkIn.MealPlanRate;
        if (dto.MealPlanId != checkIn.MealPlanId)
        {
            if (dto.MealPlanId.HasValue)
            {
                var mealPlanExists = await _context.MealPlans
                    .AnyAsync(mp => mp.MealPlanId == dto.MealPlanId.Value &&
                                   mp.IsActive &&
                                   mp.DeletedAt == null);
                if (!mealPlanExists)
                    throw new ArgumentException("Invalid meal plan ID");

                // Calculate meal plan rate directly without requiring base tariff
                var pax = await _context.Guests
                    .CountAsync(g => g.CheckInId == id && g.DeletedAt == null);

                // Query MealPlanRate table directly
                var mealPlanRate = await _context.Set<MealPlanRate>()
                    .Where(mpr => mpr.MealPlanId == dto.MealPlanId.Value &&
                                 mpr.RoomTypeId == checkIn.Room.RoomTypeId &&
                                 mpr.IsActive &&
                                 mpr.DeletedAt == null &&
                                 mpr.EffectiveFrom <= checkIn.CheckInDate &&
                                 (mpr.EffectiveTo == null || mpr.EffectiveTo >= checkIn.CheckInDate))
                    .OrderByDescending(mpr => mpr.EffectiveFrom)
                    .FirstOrDefaultAsync();

                if (mealPlanRate != null)
                {
                    // Calculate: RatePerPersonPerNight * number of guests = total meal plan rate per night
                    newMealPlanRate = mealPlanRate.RatePerPersonPerNight * pax;
                }
                else
                {
                    // If no meal plan rate is configured, set to null (will be calculated at checkout)
                    newMealPlanRate = null;
                }
            }
            else
            {
                newMealPlanRate = null;
            }
        }

        // Update fields
        checkIn.CompanyId = dto.CompanyId;
        checkIn.BusinessSourceId = dto.BusinessSourceId;
        checkIn.MealPlanId = dto.MealPlanId;
        checkIn.GuestTypeId = dto.GuestTypeId;
        checkIn.MealPlanRate = newMealPlanRate;
        if (dto.Remarks != null)
            checkIn.Remarks = dto.Remarks;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated check-in ID {Id}", id);

        // Return updated check-in without full guest details
        return new CheckInDto
        {
            Id = checkIn.Id,
            RoomId = checkIn.RoomId,
            RoomNumber = checkIn.Room.RoomNumber,
            CheckInDate = checkIn.CheckInDate,
            CheckOutDate = checkIn.CheckOutDate,
            ActualCheckInDate = checkIn.ActualCheckInDate,
            ActualCheckOutDate = checkIn.ActualCheckOutDate,
            RegistrationNo = checkIn.RegistrationNo,
            Pax = checkIn.Pax,
            Status = checkIn.Status,
            Remarks = checkIn.Remarks,
            CompanyId = checkIn.CompanyId,
            CompanyName = checkIn.Company?.CompanyName,
            BusinessSourceId = checkIn.BusinessSourceId,
            BusinessSourceName = checkIn.BusinessSource?.SourceName,
            MealPlanId = checkIn.MealPlanId,
            MealPlanName = checkIn.MealPlan?.PlanName,
            GuestTypeId = checkIn.GuestTypeId,
            GuestTypeName = checkIn.GuestType?.TypeName,
            MealPlanRate = checkIn.MealPlanRate,
            TariffApplied = checkIn.TariffApplied,
            DiscountPercentage = checkIn.DiscountPercentage,
            FinalAmount = checkIn.FinalAmount,
            IsSharedRoom = checkIn.IsSharedRoom,
            SharedGroupId = checkIn.SharedGroupId,
            CreatedAt = checkIn.CreatedAt,
            UpdatedAt = checkIn.UpdatedAt,
            CreatedBy = checkIn.CreatedBy,
            UpdatedBy = checkIn.UpdatedBy,
            GuestNames = string.Join(", ", await _context.Guests
                .Where(g => g.CheckInId == id && g.DeletedAt == null)
                .OrderBy(g => g.GuestNumber)
                .Select(g => g.GuestName)
                .ToListAsync())
        };
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
            // Check for other active check-ins on the same room
            var otherActiveCheckIns = await _context.CheckIns
                .Where(c => c.RoomId == checkIn.RoomId && c.Status == CheckInStatus.Active && c.Id != id)
                .ToListAsync();

            if (otherActiveCheckIns.Any())
            {
                // Room stays Occupied, update date range
                checkIn.Room.RoomStatusFromDate = otherActiveCheckIns.Min(c => c.CheckInDate);
                checkIn.Room.RoomStatusToDate = otherActiveCheckIns.Max(c => c.CheckOutDate);

                // If only 1 remains, un-share it
                if (otherActiveCheckIns.Count == 1)
                {
                    otherActiveCheckIns[0].IsSharedRoom = false;
                    otherActiveCheckIns[0].SharedGroupId = null;
                }
            }
            else
            {
                checkIn.Room.RoomStatus = RoomStatus.Available;
                checkIn.Room.RoomStatusFromDate = null;
                checkIn.Room.RoomStatusToDate = null;
            }
        }

        _context.CheckIns.Remove(checkIn);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted check-in ID {Id}", id);
    }

    public async Task<CheckInWithGuestsDto> CreateSharedCheckInAsync(int existingCheckInId, ShareRoomDto dto)
    {
        // Validate feature is enabled
        var isEnabled = await _systemSettingsService.IsRoomSharingEnabledAsync();
        if (!isEnabled)
            throw new BusinessRuleException("Room sharing feature is not enabled. Enable it in System Settings.");

        // Get existing check-in
        var existingCheckIn = await _context.CheckIns
            .Include(c => c.Room)
            .ThenInclude(r => r.RoomType)
            .FirstOrDefaultAsync(c => c.Id == existingCheckInId);

        if (existingCheckIn == null)
            throw new NotFoundException(nameof(CheckIn), existingCheckInId);

        if (existingCheckIn.Status != CheckInStatus.Active)
            throw new BusinessRuleException("Can only share a room with an active check-in.");

        // Validate max 3 check-ins per room
        var activeCount = await _context.CheckIns
            .CountAsync(c => c.RoomId == existingCheckIn.RoomId && c.Status == CheckInStatus.Active);
        if (activeCount >= 3)
            throw new BusinessRuleException("Maximum 3 active check-ins per room. Cannot add more.");

        // Validate dates
        if (dto.CheckOutDate <= dto.CheckInDate)
            throw new BusinessRuleException("Check-out date must be after check-in date.");

        // Validate guest count
        if (dto.Guests.Count < 1 || dto.Guests.Count > 3)
            throw new BusinessRuleException("Number of guests must be between 1 and 3.");

        var roomId = existingCheckIn.RoomId;

        // Calculate tariff independently for new guest
        decimal? tariffApplied = null;
        decimal discountPercentage = 0;
        decimal? finalAmount = null;
        decimal? mealPlanRate = null;

        var roomForTariff = existingCheckIn.Room;
        if (roomForTariff != null)
        {
            try
            {
                var tariffCalc = await _tariffService.CalculateTariffAsync(
                    roomForTariff.RoomTypeId,
                    dto.Guests.Count,
                    dto.CheckInDate,
                    dto.CheckOutDate,
                    dto.CompanyId,
                    dto.MealPlanId);

                tariffApplied = tariffCalc.ApplicableRate;
                discountPercentage = tariffCalc.DiscountPercentage;
                finalAmount = tariffCalc.MealPlanId.HasValue ? tariffCalc.TotalAmountWithMealPlan : tariffCalc.TotalAmount;
                mealPlanRate = tariffCalc.MealPlanId.HasValue ? tariffCalc.MealPlanTotalRate : null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to calculate tariff for shared check-in, proceeding without pricing");
            }
        }

        // Create tax snapshot
        var taxSnapshot = await _taxService.CreateTaxSlabSnapshotAsync(dto.ActualCheckInDate ?? DateTime.UtcNow);
        var taxSnapshotJson = JsonSerializer.Serialize(taxSnapshot);

        // Determine SharedGroupId: use existing group or the first check-in's ID
        var sharedGroupId = existingCheckIn.SharedGroupId ?? existingCheckIn.Id;

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Create new shared check-in
            var newCheckIn = new CheckIn
            {
                RoomId = roomId,
                CheckInDate = dto.CheckInDate,
                CheckOutDate = dto.CheckOutDate,
                ActualCheckInDate = dto.ActualCheckInDate ?? DateTime.UtcNow,
                RegistrationNo = dto.RegistrationNo,
                Pax = dto.Guests.Count,
                Status = CheckInStatus.Active,
                Remarks = dto.Remarks,
                CompanyId = dto.CompanyId,
                BusinessSourceId = dto.BusinessSourceId,
                MealPlanId = dto.MealPlanId,
                GuestTypeId = dto.GuestTypeId,
                MealPlanRate = mealPlanRate,
                TariffApplied = tariffApplied,
                DiscountPercentage = discountPercentage,
                FinalAmount = finalAmount,
                TaxType = dto.TaxType,
                TaxSlabSnapshotJson = taxSnapshotJson,
                IsSharedRoom = true,
                SharedGroupId = sharedGroupId,
                CreatedAt = DateTime.UtcNow
            };

            _context.CheckIns.Add(newCheckIn);
            await _context.SaveChangesAsync();

            // Create guests
            for (int i = 0; i < dto.Guests.Count; i++)
            {
                var guestDto = dto.Guests[i];
                var guest = new Guest
                {
                    CheckInId = newCheckIn.Id,
                    GuestNumber = i + 1,
                    GuestName = guestDto.GuestName,
                    Address = guestDto.Address,
                    City = guestDto.City,
                    State = guestDto.State,
                    Country = guestDto.Country,
                    MobileNo = guestDto.MobileNo,
                    PanOrAadharNo = guestDto.PanOrAadharNo,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Guests.Add(guest);
            }

            // Mark all existing active check-ins in the room as shared
            var allActiveCheckIns = await _context.CheckIns
                .Where(c => c.RoomId == roomId && c.Status == CheckInStatus.Active && c.Id != newCheckIn.Id)
                .ToListAsync();

            foreach (var ci in allActiveCheckIns)
            {
                ci.IsSharedRoom = true;
                ci.SharedGroupId = sharedGroupId;
            }

            // Update room date range to cover all active check-ins
            var room = await _context.Rooms.FindAsync(roomId);
            if (room != null)
            {
                var allDates = allActiveCheckIns.Concat(new[] { newCheckIn });
                room.RoomStatusFromDate = allDates.Min(c => c.CheckInDate);
                room.RoomStatusToDate = allDates.Max(c => c.CheckOutDate);
                // Room stays Occupied (no status change needed)
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Created shared check-in ID {Id} for room {RoomId} (group {GroupId})",
                newCheckIn.Id, roomId, sharedGroupId);

            return await GetByIdAsync(newCheckIn.Id);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<int> GetActiveCheckInCountForRoomAsync(int roomId)
    {
        return await _context.CheckIns
            .CountAsync(c => c.RoomId == roomId && c.Status == CheckInStatus.Active);
    }

    public async Task<bool> IsRoomSharingEnabledAsync()
    {
        return await _systemSettingsService.IsRoomSharingEnabledAsync();
    }
}
