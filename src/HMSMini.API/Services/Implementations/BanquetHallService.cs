using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.BanquetHall;
using HMSMini.API.Models.Entities;
using HMSMini.API.Models.Enums;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Services.Implementations;

public class BanquetHallService : IBanquetHallService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BanquetHallService> _logger;

    public BanquetHallService(ApplicationDbContext context, ILogger<BanquetHallService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<BanquetHallDto>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.BanquetHalls.Where(h => h.DeletedAt == null);
        if (!includeInactive)
            query = query.Where(h => h.IsActive);

        return await query.Select(h => new BanquetHallDto
        {
            Id = h.Id,
            HallName = h.HallName,
            MaxCapacity = h.MaxCapacity,
            MinCapacity = h.MinCapacity,
            RentPerEvent = h.RentPerEvent,
            Location = h.Location,
            Features = h.Features,
            ImagePath = h.ImagePath,
            IsActive = h.IsActive,
            CreatedAt = h.CreatedAt,
            UpdatedAt = h.UpdatedAt
        }).OrderBy(h => h.HallName).ToListAsync();
    }

    public async Task<BanquetHallDto?> GetByIdAsync(int id)
    {
        return await _context.BanquetHalls
            .Where(h => h.Id == id && h.DeletedAt == null)
            .Select(h => new BanquetHallDto
            {
                Id = h.Id,
                HallName = h.HallName,
                MaxCapacity = h.MaxCapacity,
                MinCapacity = h.MinCapacity,
                RentPerEvent = h.RentPerEvent,
                Location = h.Location,
                Features = h.Features,
                ImagePath = h.ImagePath,
                IsActive = h.IsActive,
                CreatedAt = h.CreatedAt,
                UpdatedAt = h.UpdatedAt
            }).FirstOrDefaultAsync();
    }

    public async Task<BanquetHallDto> CreateAsync(CreateBanquetHallDto dto)
    {
        var hall = new MBanquetHall
        {
            HallName = dto.HallName,
            MaxCapacity = dto.MaxCapacity,
            MinCapacity = dto.MinCapacity,
            RentPerEvent = dto.RentPerEvent,
            Location = dto.Location,
            Features = dto.Features,
            ImagePath = dto.ImagePath,
            IsActive = true
        };

        _context.BanquetHalls.Add(hall);
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(hall.Id))!;
    }

    public async Task<BanquetHallDto> UpdateAsync(int id, UpdateBanquetHallDto dto)
    {
        var hall = await _context.BanquetHalls.FindAsync(id);
        if (hall == null || hall.DeletedAt != null)
            throw new NotFoundException(nameof(MBanquetHall), id);

        hall.HallName = dto.HallName;
        hall.MaxCapacity = dto.MaxCapacity;
        hall.MinCapacity = dto.MinCapacity;
        hall.RentPerEvent = dto.RentPerEvent;
        hall.Location = dto.Location;
        hall.Features = dto.Features;
        hall.ImagePath = dto.ImagePath;

        await _context.SaveChangesAsync();
        return (await GetByIdAsync(id))!;
    }

    public async Task DeleteAsync(int id)
    {
        var hall = await _context.BanquetHalls.FindAsync(id);
        if (hall == null || hall.DeletedAt != null)
            throw new NotFoundException(nameof(MBanquetHall), id);

        hall.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<BanquetHallDto> ActivateAsync(int id)
    {
        var hall = await _context.BanquetHalls.FindAsync(id);
        if (hall == null || hall.DeletedAt != null)
            throw new NotFoundException(nameof(MBanquetHall), id);

        hall.IsActive = true;
        await _context.SaveChangesAsync();
        return (await GetByIdAsync(id))!;
    }

    public async Task<BanquetHallDto> DeactivateAsync(int id)
    {
        var hall = await _context.BanquetHalls.FindAsync(id);
        if (hall == null || hall.DeletedAt != null)
            throw new NotFoundException(nameof(MBanquetHall), id);

        hall.IsActive = false;
        await _context.SaveChangesAsync();
        return (await GetByIdAsync(id))!;
    }

    public async Task<bool> CheckAvailabilityAsync(int hallId, DateTime date, TimeSpan startTime, TimeSpan endTime, int? excludeBookingId = null)
    {
        var hasConflict = await _context.BanquetBookings
            .Where(b => b.BanquetHallId == hallId
                && b.EventDate.Date == date.Date
                && b.Status != BanquetBookingStatus.Cancelled
                && b.DeletedAt == null
                && (excludeBookingId == null || b.Id != excludeBookingId)
                && b.EventStartTime < endTime
                && b.EventEndTime > startTime)
            .AnyAsync();

        return !hasConflict;
    }
}
