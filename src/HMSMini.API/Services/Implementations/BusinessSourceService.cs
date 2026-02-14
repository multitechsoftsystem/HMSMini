using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.BusinessSource;
using HMSMini.API.Models.Entities;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Services.Implementations;

/// <summary>
/// Service for managing business sources
/// </summary>
public class BusinessSourceService : IBusinessSourceService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BusinessSourceService> _logger;

    public BusinessSourceService(ApplicationDbContext context, ILogger<BusinessSourceService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<BusinessSourceDto>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.BusinessSources
            .Where(bs => bs.DeletedAt == null);

        if (!includeInactive)
        {
            query = query.Where(bs => bs.IsActive);
        }

        return await query
            .Select(bs => new BusinessSourceDto
            {
                BusinessSourceId = bs.BusinessSourceId,
                SourceName = bs.SourceName,
                Description = bs.Description,
                IsActive = bs.IsActive,
                CreatedAt = bs.CreatedAt,
                UpdatedAt = bs.UpdatedAt
            })
            .OrderBy(bs => bs.SourceName)
            .ToListAsync();
    }

    public async Task<List<BusinessSourceDto>> GetActiveBusinessSourcesAsync()
    {
        return await _context.BusinessSources
            .Where(bs => bs.IsActive && bs.DeletedAt == null)
            .Select(bs => new BusinessSourceDto
            {
                BusinessSourceId = bs.BusinessSourceId,
                SourceName = bs.SourceName,
                Description = bs.Description,
                IsActive = bs.IsActive,
                CreatedAt = bs.CreatedAt,
                UpdatedAt = bs.UpdatedAt
            })
            .OrderBy(bs => bs.SourceName)
            .ToListAsync();
    }

    public async Task<BusinessSourceDto?> GetByIdAsync(int id)
    {
        var businessSource = await _context.BusinessSources
            .Where(bs => bs.BusinessSourceId == id && bs.DeletedAt == null)
            .Select(bs => new BusinessSourceDto
            {
                BusinessSourceId = bs.BusinessSourceId,
                SourceName = bs.SourceName,
                Description = bs.Description,
                IsActive = bs.IsActive,
                CreatedAt = bs.CreatedAt,
                UpdatedAt = bs.UpdatedAt
            })
            .FirstOrDefaultAsync();

        return businessSource;
    }

    public async Task<BusinessSourceDto> CreateAsync(CreateBusinessSourceDto dto)
    {
        // Check if business source with same name already exists
        var exists = await _context.BusinessSources
            .AnyAsync(bs => bs.SourceName == dto.SourceName && bs.DeletedAt == null);

        if (exists)
        {
            throw new InvalidOperationException($"Business source with name '{dto.SourceName}' already exists.");
        }

        var businessSource = new MBusinessSource
        {
            SourceName = dto.SourceName,
            Description = dto.Description,
            IsActive = true
        };

        _context.BusinessSources.Add(businessSource);
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(businessSource.BusinessSourceId))!;
    }

    public async Task<BusinessSourceDto> UpdateAsync(int id, UpdateBusinessSourceDto dto)
    {
        var businessSource = await _context.BusinessSources.FindAsync(id);
        if (businessSource == null || businessSource.DeletedAt != null)
            throw new NotFoundException(nameof(MBusinessSource), id);

        // Check if another business source with same name exists
        var exists = await _context.BusinessSources
            .AnyAsync(bs => bs.SourceName == dto.SourceName &&
                           bs.BusinessSourceId != id &&
                           bs.DeletedAt == null);

        if (exists)
        {
            throw new InvalidOperationException($"Business source with name '{dto.SourceName}' already exists.");
        }

        businessSource.SourceName = dto.SourceName;
        businessSource.Description = dto.Description;
        businessSource.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();

        return (await GetByIdAsync(id))!;
    }

    public async Task DeleteAsync(int id)
    {
        var businessSource = await _context.BusinessSources.FindAsync(id);
        if (businessSource == null || businessSource.DeletedAt != null)
            throw new NotFoundException(nameof(MBusinessSource), id);

        businessSource.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<BusinessSourceDto> ActivateAsync(int id)
    {
        var businessSource = await _context.BusinessSources.FindAsync(id);
        if (businessSource == null || businessSource.DeletedAt != null)
            throw new NotFoundException(nameof(MBusinessSource), id);

        businessSource.IsActive = true;
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(id))!;
    }

    public async Task<BusinessSourceDto> DeactivateAsync(int id)
    {
        var businessSource = await _context.BusinessSources.FindAsync(id);
        if (businessSource == null || businessSource.DeletedAt != null)
            throw new NotFoundException(nameof(MBusinessSource), id);

        businessSource.IsActive = false;
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(id))!;
    }
}
