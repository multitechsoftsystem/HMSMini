using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.MealPlan;
using HMSMini.API.Models.Entities;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Services.Implementations;

/// <summary>
/// Service for managing meal plans and meal plan rates
/// </summary>
public class MealPlanService : IMealPlanService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MealPlanService> _logger;

    public MealPlanService(ApplicationDbContext context, ILogger<MealPlanService> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region Meal Plan Operations

    public async Task<List<MealPlanDto>> GetAllMealPlansAsync(bool includeInactive = false)
    {
        var query = _context.Set<MMealPlan>()
            .Where(mp => mp.DeletedAt == null);

        if (!includeInactive)
        {
            query = query.Where(mp => mp.IsActive);
        }

        return await query
            .Select(mp => new MealPlanDto
            {
                MealPlanId = mp.MealPlanId,
                PlanCode = mp.PlanCode,
                PlanName = mp.PlanName,
                Description = mp.Description,
                IsActive = mp.IsActive,
                CreatedAt = mp.CreatedAt,
                UpdatedAt = mp.UpdatedAt
            })
            .OrderBy(mp => mp.PlanCode)
            .ToListAsync();
    }

    public async Task<List<MealPlanDto>> GetActiveMealPlansAsync()
    {
        return await _context.Set<MMealPlan>()
            .Where(mp => mp.IsActive && mp.DeletedAt == null)
            .Select(mp => new MealPlanDto
            {
                MealPlanId = mp.MealPlanId,
                PlanCode = mp.PlanCode,
                PlanName = mp.PlanName,
                Description = mp.Description,
                IsActive = mp.IsActive,
                CreatedAt = mp.CreatedAt,
                UpdatedAt = mp.UpdatedAt
            })
            .OrderBy(mp => mp.PlanCode)
            .ToListAsync();
    }

    public async Task<MealPlanDto?> GetMealPlanByIdAsync(int id)
    {
        var mealPlan = await _context.Set<MMealPlan>()
            .Where(mp => mp.MealPlanId == id && mp.DeletedAt == null)
            .Select(mp => new MealPlanDto
            {
                MealPlanId = mp.MealPlanId,
                PlanCode = mp.PlanCode,
                PlanName = mp.PlanName,
                Description = mp.Description,
                IsActive = mp.IsActive,
                CreatedAt = mp.CreatedAt,
                UpdatedAt = mp.UpdatedAt
            })
            .FirstOrDefaultAsync();

        return mealPlan;
    }

    public async Task<MealPlanDto> CreateMealPlanAsync(CreateMealPlanDto dto)
    {
        // Check if meal plan with same code already exists
        var exists = await _context.Set<MMealPlan>()
            .AnyAsync(mp => mp.PlanCode == dto.PlanCode && mp.DeletedAt == null);

        if (exists)
        {
            throw new InvalidOperationException($"Meal plan with code '{dto.PlanCode}' already exists.");
        }

        var mealPlan = new MMealPlan
        {
            PlanCode = dto.PlanCode,
            PlanName = dto.PlanName,
            Description = dto.Description,
            IsActive = true
        };

        _context.Set<MMealPlan>().Add(mealPlan);
        await _context.SaveChangesAsync();

        return (await GetMealPlanByIdAsync(mealPlan.MealPlanId))!;
    }

    public async Task<MealPlanDto> UpdateMealPlanAsync(int id, UpdateMealPlanDto dto)
    {
        var mealPlan = await _context.Set<MMealPlan>().FindAsync(id);
        if (mealPlan == null || mealPlan.DeletedAt != null)
            throw new NotFoundException(nameof(MMealPlan), id);

        // Check if another meal plan with same code exists
        var exists = await _context.Set<MMealPlan>()
            .AnyAsync(mp => mp.PlanCode == dto.PlanCode &&
                           mp.MealPlanId != id &&
                           mp.DeletedAt == null);

        if (exists)
        {
            throw new InvalidOperationException($"Meal plan with code '{dto.PlanCode}' already exists.");
        }

        mealPlan.PlanCode = dto.PlanCode;
        mealPlan.PlanName = dto.PlanName;
        mealPlan.Description = dto.Description;
        mealPlan.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();

        return (await GetMealPlanByIdAsync(id))!;
    }

    public async Task DeleteMealPlanAsync(int id)
    {
        var mealPlan = await _context.Set<MMealPlan>().FindAsync(id);
        if (mealPlan == null || mealPlan.DeletedAt != null)
            throw new NotFoundException(nameof(MMealPlan), id);

        mealPlan.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<MealPlanDto> ActivateMealPlanAsync(int id)
    {
        var mealPlan = await _context.Set<MMealPlan>().FindAsync(id);
        if (mealPlan == null || mealPlan.DeletedAt != null)
            throw new NotFoundException(nameof(MMealPlan), id);

        mealPlan.IsActive = true;
        await _context.SaveChangesAsync();

        return (await GetMealPlanByIdAsync(id))!;
    }

    public async Task<MealPlanDto> DeactivateMealPlanAsync(int id)
    {
        var mealPlan = await _context.Set<MMealPlan>().FindAsync(id);
        if (mealPlan == null || mealPlan.DeletedAt != null)
            throw new NotFoundException(nameof(MMealPlan), id);

        mealPlan.IsActive = false;
        await _context.SaveChangesAsync();

        return (await GetMealPlanByIdAsync(id))!;
    }

    #endregion

    #region Meal Plan Rate Operations

    public async Task<List<MealPlanRateDto>> GetAllMealPlanRatesAsync(bool includeInactive = false)
    {
        var query = _context.Set<MealPlanRate>()
            .Include(mpr => mpr.MealPlan)
            .Include(mpr => mpr.RoomType)
            .Where(mpr => mpr.DeletedAt == null);

        if (!includeInactive)
        {
            query = query.Where(mpr => mpr.IsActive);
        }

        return await query
            .Select(mpr => new MealPlanRateDto
            {
                Id = mpr.Id,
                MealPlanId = mpr.MealPlanId,
                MealPlanName = mpr.MealPlan.PlanName,
                MealPlanCode = mpr.MealPlan.PlanCode,
                RoomTypeId = mpr.RoomTypeId,
                RoomTypeName = mpr.RoomType.RoomType,
                RatePerPersonPerNight = mpr.RatePerPersonPerNight,
                EffectiveFrom = mpr.EffectiveFrom,
                EffectiveTo = mpr.EffectiveTo,
                IsActive = mpr.IsActive,
                CreatedAt = mpr.CreatedAt,
                UpdatedAt = mpr.UpdatedAt
            })
            .OrderBy(mpr => mpr.MealPlanCode)
            .ThenBy(mpr => mpr.RoomTypeName)
            .ThenByDescending(mpr => mpr.EffectiveFrom)
            .ToListAsync();
    }

    public async Task<List<MealPlanRateDto>> GetMealPlanRatesByMealPlanIdAsync(int mealPlanId)
    {
        return await _context.Set<MealPlanRate>()
            .Include(mpr => mpr.MealPlan)
            .Include(mpr => mpr.RoomType)
            .Where(mpr => mpr.MealPlanId == mealPlanId && mpr.DeletedAt == null)
            .Select(mpr => new MealPlanRateDto
            {
                Id = mpr.Id,
                MealPlanId = mpr.MealPlanId,
                MealPlanName = mpr.MealPlan.PlanName,
                MealPlanCode = mpr.MealPlan.PlanCode,
                RoomTypeId = mpr.RoomTypeId,
                RoomTypeName = mpr.RoomType.RoomType,
                RatePerPersonPerNight = mpr.RatePerPersonPerNight,
                EffectiveFrom = mpr.EffectiveFrom,
                EffectiveTo = mpr.EffectiveTo,
                IsActive = mpr.IsActive,
                CreatedAt = mpr.CreatedAt,
                UpdatedAt = mpr.UpdatedAt
            })
            .OrderBy(mpr => mpr.RoomTypeName)
            .ThenByDescending(mpr => mpr.EffectiveFrom)
            .ToListAsync();
    }

    public async Task<List<MealPlanRateDto>> GetMealPlanRatesByRoomTypeIdAsync(int roomTypeId)
    {
        return await _context.Set<MealPlanRate>()
            .Include(mpr => mpr.MealPlan)
            .Include(mpr => mpr.RoomType)
            .Where(mpr => mpr.RoomTypeId == roomTypeId && mpr.DeletedAt == null)
            .Select(mpr => new MealPlanRateDto
            {
                Id = mpr.Id,
                MealPlanId = mpr.MealPlanId,
                MealPlanName = mpr.MealPlan.PlanName,
                MealPlanCode = mpr.MealPlan.PlanCode,
                RoomTypeId = mpr.RoomTypeId,
                RoomTypeName = mpr.RoomType.RoomType,
                RatePerPersonPerNight = mpr.RatePerPersonPerNight,
                EffectiveFrom = mpr.EffectiveFrom,
                EffectiveTo = mpr.EffectiveTo,
                IsActive = mpr.IsActive,
                CreatedAt = mpr.CreatedAt,
                UpdatedAt = mpr.UpdatedAt
            })
            .OrderBy(mpr => mpr.MealPlanCode)
            .ThenByDescending(mpr => mpr.EffectiveFrom)
            .ToListAsync();
    }

    public async Task<MealPlanRateDto?> GetMealPlanRateByIdAsync(int id)
    {
        var rate = await _context.Set<MealPlanRate>()
            .Include(mpr => mpr.MealPlan)
            .Include(mpr => mpr.RoomType)
            .Where(mpr => mpr.Id == id && mpr.DeletedAt == null)
            .Select(mpr => new MealPlanRateDto
            {
                Id = mpr.Id,
                MealPlanId = mpr.MealPlanId,
                MealPlanName = mpr.MealPlan.PlanName,
                MealPlanCode = mpr.MealPlan.PlanCode,
                RoomTypeId = mpr.RoomTypeId,
                RoomTypeName = mpr.RoomType.RoomType,
                RatePerPersonPerNight = mpr.RatePerPersonPerNight,
                EffectiveFrom = mpr.EffectiveFrom,
                EffectiveTo = mpr.EffectiveTo,
                IsActive = mpr.IsActive,
                CreatedAt = mpr.CreatedAt,
                UpdatedAt = mpr.UpdatedAt
            })
            .FirstOrDefaultAsync();

        return rate;
    }

    public async Task<MealPlanRateDto> CreateMealPlanRateAsync(CreateMealPlanRateDto dto)
    {
        // Verify meal plan exists
        var mealPlanExists = await _context.Set<MMealPlan>()
            .AnyAsync(mp => mp.MealPlanId == dto.MealPlanId && mp.DeletedAt == null);
        if (!mealPlanExists)
            throw new NotFoundException(nameof(MMealPlan), dto.MealPlanId);

        // Verify room type exists
        var roomTypeExists = await _context.RoomTypes
            .AnyAsync(rt => rt.RoomTypeId == dto.RoomTypeId);
        if (!roomTypeExists)
            throw new NotFoundException(nameof(MRoomType), dto.RoomTypeId);

        var rate = new MealPlanRate
        {
            MealPlanId = dto.MealPlanId,
            RoomTypeId = dto.RoomTypeId,
            RatePerPersonPerNight = dto.RatePerPersonPerNight,
            EffectiveFrom = dto.EffectiveFrom,
            EffectiveTo = dto.EffectiveTo,
            IsActive = true
        };

        _context.Set<MealPlanRate>().Add(rate);
        await _context.SaveChangesAsync();

        return (await GetMealPlanRateByIdAsync(rate.Id))!;
    }

    public async Task<MealPlanRateDto> UpdateMealPlanRateAsync(int id, UpdateMealPlanRateDto dto)
    {
        var rate = await _context.Set<MealPlanRate>().FindAsync(id);
        if (rate == null || rate.DeletedAt != null)
            throw new NotFoundException(nameof(MealPlanRate), id);

        // Verify meal plan exists
        var mealPlanExists = await _context.Set<MMealPlan>()
            .AnyAsync(mp => mp.MealPlanId == dto.MealPlanId && mp.DeletedAt == null);
        if (!mealPlanExists)
            throw new NotFoundException(nameof(MMealPlan), dto.MealPlanId);

        // Verify room type exists
        var roomTypeExists = await _context.RoomTypes
            .AnyAsync(rt => rt.RoomTypeId == dto.RoomTypeId);
        if (!roomTypeExists)
            throw new NotFoundException(nameof(MRoomType), dto.RoomTypeId);

        rate.MealPlanId = dto.MealPlanId;
        rate.RoomTypeId = dto.RoomTypeId;
        rate.RatePerPersonPerNight = dto.RatePerPersonPerNight;
        rate.EffectiveFrom = dto.EffectiveFrom;
        rate.EffectiveTo = dto.EffectiveTo;
        rate.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();

        return (await GetMealPlanRateByIdAsync(id))!;
    }

    public async Task DeleteMealPlanRateAsync(int id)
    {
        var rate = await _context.Set<MealPlanRate>().FindAsync(id);
        if (rate == null || rate.DeletedAt != null)
            throw new NotFoundException(nameof(MealPlanRate), id);

        rate.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    #endregion

    #region Utility Methods

    public async Task<decimal?> GetMealPlanRateForBookingAsync(int mealPlanId, int roomTypeId, DateTime checkInDate)
    {
        var rate = await _context.Set<MealPlanRate>()
            .Where(mpr => mpr.MealPlanId == mealPlanId &&
                         mpr.RoomTypeId == roomTypeId &&
                         mpr.IsActive &&
                         mpr.DeletedAt == null &&
                         mpr.EffectiveFrom <= checkInDate &&
                         (mpr.EffectiveTo == null || mpr.EffectiveTo >= checkInDate))
            .OrderByDescending(mpr => mpr.EffectiveFrom)
            .FirstOrDefaultAsync();

        return rate?.RatePerPersonPerNight;
    }

    #endregion
}
