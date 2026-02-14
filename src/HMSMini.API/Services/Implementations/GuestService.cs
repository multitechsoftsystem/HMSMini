using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.Guest;
using HMSMini.API.Models.Entities;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Services.Implementations;

public class GuestService : IGuestService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GuestService> _logger;

    public GuestService(ApplicationDbContext context, ILogger<GuestService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<GuestDto> GetByIdAsync(int id)
    {
        var guest = await _context.Guests.FindAsync(id);
        if (guest == null)
            throw new NotFoundException(nameof(Guest), id);

        return new GuestDto
        {
            Id = guest.Id,
            CheckInId = guest.CheckInId,
            GuestNumber = guest.GuestNumber,
            GuestName = guest.GuestName,
            Address = guest.Address,
            City = guest.City,
            State = guest.State,
            Country = guest.Country,
            MobileNo = guest.MobileNo,
            PanOrAadharNo = guest.PanOrAadharNo,
            Photo1Path = guest.Photo1Path,
            Photo2Path = guest.Photo2Path,
            ActualCheckInDate = guest.ActualCheckInDate,
            ActualCheckOutDate = guest.ActualCheckOutDate,
            Status = (int)guest.Status,
            CreatedAt = guest.CreatedAt,
            UpdatedAt = guest.UpdatedAt,
            CreatedBy = guest.CreatedBy,
            UpdatedBy = guest.UpdatedBy
        };
    }

    public async Task<List<GuestDto>> GetByCheckInIdAsync(int checkInId)
    {
        return await _context.Guests
            .Where(g => g.CheckInId == checkInId)
            .Select(g => new GuestDto
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
                ActualCheckInDate = g.ActualCheckInDate,
                ActualCheckOutDate = g.ActualCheckOutDate,
                Status = (int)g.Status,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt,
                CreatedBy = g.CreatedBy,
                UpdatedBy = g.UpdatedBy
            })
            .OrderBy(g => g.GuestNumber)
            .ToListAsync();
    }

    public async Task<GuestDto> CreateAsync(int checkInId, CreateGuestDto dto)
    {
        // Verify check-in exists and is active
        var checkIn = await _context.CheckIns.FindAsync(checkInId);
        if (checkIn == null)
            throw new NotFoundException(nameof(CheckIn), checkInId);

        if (checkIn.Status != Models.Enums.CheckInStatus.Active)
            throw new BusinessRuleException("Cannot add guests to a checked-out reservation");

        // Get all active guests for this check-in
        var allGuests = await _context.Guests
            .Where(g => g.CheckInId == checkInId)
            .ToListAsync();

        // Find the first available guest number (either a checked-out slot or next number)
        int assignedGuestNumber;

        // Check if Guest 2 slot is available (either no active Guest 2 exists, or Guest 2 is checked out)
        var guest2 = allGuests.FirstOrDefault(g => g.GuestNumber == 2 && g.Status == Models.Enums.GuestStatus.Active);
        if (guest2 == null)
        {
            assignedGuestNumber = 2;
        }
        // Check if Guest 3 slot is available
        else
        {
            var guest3 = allGuests.FirstOrDefault(g => g.GuestNumber == 3 && g.Status == Models.Enums.GuestStatus.Active);
            if (guest3 == null)
            {
                assignedGuestNumber = 3;
            }
            else
            {
                throw new BusinessRuleException("Maximum 3 active guests allowed per check-in");
            }
        }

        // Create new guest with assigned guest number
        var newGuest = new Guest
        {
            CheckInId = checkInId,
            GuestNumber = assignedGuestNumber,
            GuestName = dto.GuestName,
            Address = dto.Address,
            City = dto.City,
            State = dto.State,
            Country = dto.Country,
            MobileNo = dto.MobileNo,
            PanOrAadharNo = dto.PanOrAadharNo,
            ActualCheckInDate = DateTime.UtcNow,
            Status = Models.Enums.GuestStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        _context.Guests.Add(newGuest);

        // Update PAX count to reflect active guests only
        var activeGuestCount = allGuests.Count(g => g.Status == Models.Enums.GuestStatus.Active) + 1; // Add the new guest

        checkIn.Pax = activeGuestCount;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Added new guest (Guest #{GuestNumber}) to check-in ID {CheckInId}", newGuest.GuestNumber, checkInId);

        return await GetByIdAsync(newGuest.Id);
    }

    public async Task<GuestDto> UpdateAsync(int id, CreateGuestDto dto)
    {
        var guest = await _context.Guests.FindAsync(id);
        if (guest == null)
            throw new NotFoundException(nameof(Guest), id);

        guest.GuestName = dto.GuestName;
        guest.Address = dto.Address;
        guest.City = dto.City;
        guest.State = dto.State;
        guest.Country = dto.Country;
        guest.MobileNo = dto.MobileNo;
        guest.PanOrAadharNo = dto.PanOrAadharNo;
        guest.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated guest ID {Id}", id);

        return await GetByIdAsync(id);
    }

    public async Task<GuestDto> UpdatePhotoPathAsync(int id, int photoNumber, string photoPath)
    {
        var guest = await _context.Guests.FindAsync(id);
        if (guest == null)
            throw new NotFoundException(nameof(Guest), id);

        if (photoNumber == 1)
            guest.Photo1Path = photoPath;
        else if (photoNumber == 2)
            guest.Photo2Path = photoPath;
        else
            throw new BusinessRuleException("Photo number must be 1 or 2");

        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated photo {PhotoNumber} path for guest ID {Id}", photoNumber, id);

        return await GetByIdAsync(id);
    }

    public async Task<GuestDto> CheckOutGuestAsync(int id)
    {
        var guest = await _context.Guests.FindAsync(id);
        if (guest == null)
            throw new NotFoundException(nameof(Guest), id);

        if (guest.Status == Models.Enums.GuestStatus.CheckedOut)
            throw new BusinessRuleException("Guest has already been checked out");

        if (guest.GuestNumber == 1)
            throw new BusinessRuleException("Cannot check out the primary guest. Use the main check-out function instead.");

        guest.ActualCheckOutDate = DateTime.UtcNow;
        guest.Status = Models.Enums.GuestStatus.CheckedOut;
        guest.UpdatedAt = DateTime.UtcNow;

        // Update PAX count on check-in to reflect active guests only
        var checkIn = await _context.CheckIns.FindAsync(guest.CheckInId);
        if (checkIn != null)
        {
            var activeGuestCount = await _context.Guests
                .Where(g => g.CheckInId == guest.CheckInId && g.Status == Models.Enums.GuestStatus.Active)
                .CountAsync();

            checkIn.Pax = activeGuestCount - 1; // Subtract the guest being checked out
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Checked out guest ID {Id} (Guest #{GuestNumber})", id, guest.GuestNumber);

        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        var guest = await _context.Guests.FindAsync(id);
        if (guest == null)
            throw new NotFoundException(nameof(Guest), id);

        _context.Guests.Remove(guest);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted guest ID {Id}", id);
    }

    public async Task<List<GuestDto>> SearchGuestsByMobileAsync(string mobile)
    {
        if (string.IsNullOrWhiteSpace(mobile))
            return new List<GuestDto>();

        var guests = await _context.Guests
            .Where(g => g.MobileNo != null && g.MobileNo.Contains(mobile))
            .OrderByDescending(g => g.CreatedAt)
            .Take(30)
            .Select(g => new GuestDto
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
                ActualCheckInDate = g.ActualCheckInDate,
                ActualCheckOutDate = g.ActualCheckOutDate,
                Status = (int)g.Status,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt,
                CreatedBy = g.CreatedBy,
                UpdatedBy = g.UpdatedBy
            })
            .ToListAsync();

        // Deduplicate by name+mobile combination and take most recent
        var uniqueGuests = guests
            .GroupBy(g => new { g.GuestName, g.MobileNo })
            .Select(group => group.First())
            .ToList();

        return uniqueGuests;
    }

    public async Task<List<GuestDto>> SearchGuestsByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new List<GuestDto>();

        var guests = await _context.Guests
            .Where(g => EF.Functions.Like(g.GuestName, $"%{name}%"))
            .OrderByDescending(g => g.CreatedAt)
            .Take(30)
            .Select(g => new GuestDto
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
                ActualCheckInDate = g.ActualCheckInDate,
                ActualCheckOutDate = g.ActualCheckOutDate,
                Status = (int)g.Status,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt,
                CreatedBy = g.CreatedBy,
                UpdatedBy = g.UpdatedBy
            })
            .ToListAsync();

        // Deduplicate by name+mobile combination and take most recent
        var uniqueGuests = guests
            .GroupBy(g => new { g.GuestName, g.MobileNo })
            .Select(group => group.First())
            .ToList();

        return uniqueGuests;
    }

    public async Task<List<GuestDto>> SearchGuestsAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new List<GuestDto>();

        // Try to determine if query looks like a mobile number (contains only digits)
        var isNumeric = query.All(char.IsDigit);

        var guests = await _context.Guests
            .Where(g =>
                (isNumeric && g.MobileNo != null && g.MobileNo.Contains(query)) ||
                (!isNumeric && EF.Functions.Like(g.GuestName, $"%{query}%")) ||
                (g.MobileNo != null && g.MobileNo.Contains(query)) ||
                EF.Functions.Like(g.GuestName, $"%{query}%"))
            .OrderByDescending(g => g.CreatedAt)
            .Take(30)
            .Select(g => new GuestDto
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
                ActualCheckInDate = g.ActualCheckInDate,
                ActualCheckOutDate = g.ActualCheckOutDate,
                Status = (int)g.Status,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt,
                CreatedBy = g.CreatedBy,
                UpdatedBy = g.UpdatedBy
            })
            .ToListAsync();

        // Deduplicate by name+mobile combination and take most recent
        var uniqueGuests = guests
            .GroupBy(g => new { g.GuestName, g.MobileNo })
            .Select(group => group.First())
            .ToList();

        return uniqueGuests;
    }
}
