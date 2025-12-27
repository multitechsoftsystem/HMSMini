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
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt,
                CreatedBy = g.CreatedBy,
                UpdatedBy = g.UpdatedBy
            })
            .OrderBy(g => g.GuestNumber)
            .ToListAsync();
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

    public async Task DeleteAsync(int id)
    {
        var guest = await _context.Guests.FindAsync(id);
        if (guest == null)
            throw new NotFoundException(nameof(Guest), id);

        _context.Guests.Remove(guest);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted guest ID {Id}", id);
    }
}
