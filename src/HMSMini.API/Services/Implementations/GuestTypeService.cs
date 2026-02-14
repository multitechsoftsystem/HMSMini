using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.GuestType;
using HMSMini.API.Models.Entities;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Services.Implementations;

public class GuestTypeService : IGuestTypeService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GuestTypeService> _logger;

    public GuestTypeService(ApplicationDbContext context, ILogger<GuestTypeService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<GuestTypeDto>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.MGuestTypes
            .Where(gt => gt.DeletedAt == null);

        if (!includeInactive)
        {
            query = query.Where(gt => gt.IsActive);
        }

        return await query
            .OrderBy(gt => gt.DisplayOrder)
            .Select(gt => new GuestTypeDto
            {
                GuestTypeId = gt.GuestTypeId,
                TypeName = gt.TypeName,
                Description = gt.Description,
                IsActive = gt.IsActive,
                DisplayOrder = gt.DisplayOrder,
                CreatedAt = gt.CreatedAt,
                UpdatedAt = gt.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<List<GuestTypeDto>> GetActiveAsync()
    {
        return await GetAllAsync(includeInactive: false);
    }

    public async Task<GuestTypeDto?> GetByIdAsync(int id)
    {
        var guestType = await _context.MGuestTypes
            .Where(gt => gt.GuestTypeId == id && gt.DeletedAt == null)
            .Select(gt => new GuestTypeDto
            {
                GuestTypeId = gt.GuestTypeId,
                TypeName = gt.TypeName,
                Description = gt.Description,
                IsActive = gt.IsActive,
                DisplayOrder = gt.DisplayOrder,
                CreatedAt = gt.CreatedAt,
                UpdatedAt = gt.UpdatedAt
            })
            .FirstOrDefaultAsync();

        return guestType;
    }

    public async Task<GuestTypeDto> CreateAsync(CreateGuestTypeDto dto)
    {
        var guestType = new MGuestType
        {
            TypeName = dto.TypeName,
            Description = dto.Description,
            DisplayOrder = dto.DisplayOrder,
            IsActive = true
        };

        _context.MGuestTypes.Add(guestType);
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(guestType.GuestTypeId))!;
    }

    public async Task<GuestTypeDto> UpdateAsync(int id, UpdateGuestTypeDto dto)
    {
        var guestType = await _context.MGuestTypes.FindAsync(id);
        if (guestType == null || guestType.DeletedAt != null)
            throw new NotFoundException(nameof(MGuestType), id);

        guestType.TypeName = dto.TypeName;
        guestType.Description = dto.Description;
        guestType.DisplayOrder = dto.DisplayOrder;
        guestType.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return (await GetByIdAsync(id))!;
    }

    public async Task DeleteAsync(int id)
    {
        var guestType = await _context.MGuestTypes.FindAsync(id);
        if (guestType == null || guestType.DeletedAt != null)
            throw new NotFoundException(nameof(MGuestType), id);

        guestType.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<GuestTypeDto> ActivateAsync(int id)
    {
        var guestType = await _context.MGuestTypes.FindAsync(id);
        if (guestType == null || guestType.DeletedAt != null)
            throw new NotFoundException(nameof(MGuestType), id);

        guestType.IsActive = true;
        guestType.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(id))!;
    }

    public async Task<GuestTypeDto> DeactivateAsync(int id)
    {
        var guestType = await _context.MGuestTypes.FindAsync(id);
        if (guestType == null || guestType.DeletedAt != null)
            throw new NotFoundException(nameof(MGuestType), id);

        guestType.IsActive = false;
        guestType.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(id))!;
    }
}
