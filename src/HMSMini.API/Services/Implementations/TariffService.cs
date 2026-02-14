using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.Tariff;
using HMSMini.API.Models.Entities;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Services.Implementations;

public class TariffService : ITariffService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TariffService> _logger;

    public TariffService(ApplicationDbContext context, ILogger<TariffService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<TariffCalculationDto> CalculateTariffAsync(
        int roomTypeId,
        int occupancyCount,
        DateTime checkInDate,
        DateTime checkOutDate,
        int? companyId = null,
        int? mealPlanId = null)
    {
        // Get room type
        var roomType = await _context.RoomTypes.FindAsync(roomTypeId);
        if (roomType == null)
            throw new NotFoundException(nameof(MRoomType), roomTypeId);

        // Calculate number of nights
        var nights = (checkOutDate.Date - checkInDate.Date).Days;
        if (nights <= 0)
            throw new BusinessRuleException("Check-out date must be after check-in date");

        // Initialize result
        var result = new TariffCalculationDto
        {
            RoomTypeId = roomTypeId,
            RoomTypeName = roomType.RoomType,
            OccupancyCount = occupancyCount,
            NumberOfNights = nights,
            DiscountPercentage = 0
        };

        // Get base tariff
        var baseTariff = await _context.BaseTariffs
            .Where(bt => bt.RoomTypeId == roomTypeId &&
                        bt.OccupancyCount == occupancyCount &&
                        bt.IsActive &&
                        bt.DeletedAt == null &&
                        bt.EffectiveFrom <= checkInDate &&
                        (bt.EffectiveTo == null || bt.EffectiveTo >= checkInDate))
            .OrderByDescending(bt => bt.EffectiveFrom)
            .FirstOrDefaultAsync();

        if (baseTariff == null)
            throw new BusinessRuleException($"No base tariff found for {roomType.RoomType} with {occupancyCount} occupant(s)");

        result.BaseRate = baseTariff.RatePerNight;
        result.ApplicableRate = baseTariff.RatePerNight;
        result.TariffSource = "Base";

        // If company is specified, check for company-specific tariff or discount
        if (companyId.HasValue)
        {
            var company = await _context.Companies.FindAsync(companyId.Value);
            if (company == null)
                throw new NotFoundException(nameof(Company), companyId.Value);

            // Check for company-specific tariff
            var companyTariff = await _context.CompanyTariffs
                .Where(ct => ct.CompanyId == companyId.Value &&
                            ct.RoomTypeId == roomTypeId &&
                            ct.OccupancyCount == occupancyCount &&
                            ct.IsActive &&
                            ct.DeletedAt == null &&
                            ct.EffectiveFrom <= checkInDate &&
                            (ct.EffectiveTo == null || ct.EffectiveTo >= checkInDate))
                .OrderByDescending(ct => ct.EffectiveFrom)
                .FirstOrDefaultAsync();

            if (companyTariff != null)
            {
                result.CompanyRate = companyTariff.RatePerNight;
                result.ApplicableRate = companyTariff.RatePerNight;
                result.TariffSource = "Company";
            }

            // Apply company discount
            result.DiscountPercentage = company.DiscountPercentage;
            if (result.TariffSource == "Company")
            {
                result.TariffSource = "CompanyWithDiscount";
            }
        }

        // Calculate final rate and total amount
        result.FinalRate = result.ApplicableRate * (1 - result.DiscountPercentage / 100);
        result.TotalAmount = result.FinalRate * nights;

        // Calculate meal plan charges if specified
        if (mealPlanId.HasValue)
        {
            var mealPlanRate = await _context.Set<MealPlanRate>()
                .Include(mpr => mpr.MealPlan)
                .Where(mpr => mpr.MealPlanId == mealPlanId.Value &&
                             mpr.RoomTypeId == roomTypeId &&
                             mpr.IsActive &&
                             mpr.DeletedAt == null &&
                             mpr.EffectiveFrom <= checkInDate &&
                             (mpr.EffectiveTo == null || mpr.EffectiveTo >= checkInDate))
                .OrderByDescending(mpr => mpr.EffectiveFrom)
                .FirstOrDefaultAsync();

            if (mealPlanRate != null)
            {
                decimal mealPlanRatePerPerson = mealPlanRate.RatePerPersonPerNight;
                decimal mealPlanTotalRate = mealPlanRatePerPerson * occupancyCount;
                decimal totalMealPlanAmount = mealPlanTotalRate * nights;

                result.MealPlanId = mealPlanId.Value;
                result.MealPlanName = mealPlanRate.MealPlan.PlanName;
                result.MealPlanRatePerPerson = mealPlanRatePerPerson;
                result.MealPlanTotalRate = mealPlanTotalRate;
                result.TotalMealPlanAmount = totalMealPlanAmount;
                result.BaseRateWithMealPlan = result.FinalRate + mealPlanTotalRate;
                result.TotalAmountWithMealPlan = result.TotalAmount + totalMealPlanAmount;
            }
        }

        return result;
    }

    public async Task<List<TariffCalculationDto>> CalculateAllRoomTypesAsync(
        DateTime checkInDate,
        DateTime checkOutDate,
        int? companyId = null,
        int? mealPlanId = null)
    {
        var roomTypes = await _context.RoomTypes.Where(rt => rt.DeletedAt == null).ToListAsync();
        var results = new List<TariffCalculationDto>();

        foreach (var roomType in roomTypes)
        {
            // Calculate for occupancy level 2 (most common)
            try
            {
                var calc = await CalculateTariffAsync(roomType.RoomTypeId, 2, checkInDate, checkOutDate, companyId, mealPlanId);
                results.Add(calc);
            }
            catch (BusinessRuleException)
            {
                // Skip if no tariff found for this room type
                continue;
            }
        }

        return results;
    }

    // Base Tariff Management
    public async Task<List<BaseTariffDto>> GetBaseTariffsAsync()
    {
        return await _context.BaseTariffs
            .Include(bt => bt.RoomType)
            .Where(bt => bt.DeletedAt == null)
            .Select(bt => new BaseTariffDto
            {
                Id = bt.Id,
                RoomTypeId = bt.RoomTypeId,
                RoomTypeName = bt.RoomType.RoomType,
                OccupancyCount = bt.OccupancyCount,
                RatePerNight = bt.RatePerNight,
                EffectiveFrom = bt.EffectiveFrom,
                EffectiveTo = bt.EffectiveTo,
                IsActive = bt.IsActive
            })
            .ToListAsync();
    }

    public async Task<BaseTariffDto?> GetBaseTariffByIdAsync(int id)
    {
        var tariff = await _context.BaseTariffs
            .Include(bt => bt.RoomType)
            .Where(bt => bt.Id == id && bt.DeletedAt == null)
            .Select(bt => new BaseTariffDto
            {
                Id = bt.Id,
                RoomTypeId = bt.RoomTypeId,
                RoomTypeName = bt.RoomType.RoomType,
                OccupancyCount = bt.OccupancyCount,
                RatePerNight = bt.RatePerNight,
                EffectiveFrom = bt.EffectiveFrom,
                EffectiveTo = bt.EffectiveTo,
                IsActive = bt.IsActive
            })
            .FirstOrDefaultAsync();

        return tariff;
    }

    public async Task<List<BaseTariffDto>> GetBaseTariffsByRoomTypeAsync(int roomTypeId)
    {
        return await _context.BaseTariffs
            .Include(bt => bt.RoomType)
            .Where(bt => bt.RoomTypeId == roomTypeId && bt.DeletedAt == null)
            .Select(bt => new BaseTariffDto
            {
                Id = bt.Id,
                RoomTypeId = bt.RoomTypeId,
                RoomTypeName = bt.RoomType.RoomType,
                OccupancyCount = bt.OccupancyCount,
                RatePerNight = bt.RatePerNight,
                EffectiveFrom = bt.EffectiveFrom,
                EffectiveTo = bt.EffectiveTo,
                IsActive = bt.IsActive
            })
            .ToListAsync();
    }

    public async Task<BaseTariffDto> CreateBaseTariffAsync(CreateBaseTariffDto dto)
    {
        var tariff = new BaseTariff
        {
            RoomTypeId = dto.RoomTypeId,
            OccupancyCount = dto.OccupancyCount,
            RatePerNight = dto.RatePerNight,
            EffectiveFrom = dto.EffectiveFrom,
            EffectiveTo = dto.EffectiveTo,
            IsActive = true
        };

        _context.BaseTariffs.Add(tariff);
        await _context.SaveChangesAsync();

        return (await GetBaseTariffByIdAsync(tariff.Id))!;
    }

    public async Task<BaseTariffDto> UpdateBaseTariffAsync(int id, UpdateBaseTariffDto dto)
    {
        var tariff = await _context.BaseTariffs.FindAsync(id);
        if (tariff == null || tariff.DeletedAt != null)
            throw new NotFoundException(nameof(BaseTariff), id);

        tariff.RoomTypeId = dto.RoomTypeId;
        tariff.OccupancyCount = dto.OccupancyCount;
        tariff.RatePerNight = dto.RatePerNight;
        tariff.EffectiveFrom = dto.EffectiveFrom;
        tariff.EffectiveTo = dto.EffectiveTo;

        await _context.SaveChangesAsync();

        return (await GetBaseTariffByIdAsync(id))!;
    }

    public async Task DeleteBaseTariffAsync(int id)
    {
        var tariff = await _context.BaseTariffs.FindAsync(id);
        if (tariff == null || tariff.DeletedAt != null)
            throw new NotFoundException(nameof(BaseTariff), id);

        tariff.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    // Company Tariff Management
    public async Task<List<CompanyTariffDto>> GetCompanyTariffsAsync(int companyId)
    {
        return await _context.CompanyTariffs
            .Include(ct => ct.Company)
            .Include(ct => ct.RoomType)
            .Where(ct => ct.CompanyId == companyId && ct.DeletedAt == null)
            .Select(ct => new CompanyTariffDto
            {
                Id = ct.Id,
                CompanyId = ct.CompanyId,
                CompanyName = ct.Company.CompanyName,
                RoomTypeId = ct.RoomTypeId,
                RoomTypeName = ct.RoomType.RoomType,
                OccupancyCount = ct.OccupancyCount,
                RatePerNight = ct.RatePerNight,
                EffectiveFrom = ct.EffectiveFrom,
                EffectiveTo = ct.EffectiveTo,
                IsActive = ct.IsActive
            })
            .ToListAsync();
    }

    public async Task<CompanyTariffDto?> GetCompanyTariffByIdAsync(int id)
    {
        return await _context.CompanyTariffs
            .Include(ct => ct.Company)
            .Include(ct => ct.RoomType)
            .Where(ct => ct.Id == id && ct.DeletedAt == null)
            .Select(ct => new CompanyTariffDto
            {
                Id = ct.Id,
                CompanyId = ct.CompanyId,
                CompanyName = ct.Company.CompanyName,
                RoomTypeId = ct.RoomTypeId,
                RoomTypeName = ct.RoomType.RoomType,
                OccupancyCount = ct.OccupancyCount,
                RatePerNight = ct.RatePerNight,
                EffectiveFrom = ct.EffectiveFrom,
                EffectiveTo = ct.EffectiveTo,
                IsActive = ct.IsActive
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CompanyTariffDto> CreateCompanyTariffAsync(CreateCompanyTariffDto dto)
    {
        var tariff = new CompanyTariff
        {
            CompanyId = dto.CompanyId,
            RoomTypeId = dto.RoomTypeId,
            OccupancyCount = dto.OccupancyCount,
            RatePerNight = dto.RatePerNight,
            EffectiveFrom = dto.EffectiveFrom,
            EffectiveTo = dto.EffectiveTo,
            IsActive = true
        };

        _context.CompanyTariffs.Add(tariff);
        await _context.SaveChangesAsync();

        return (await GetCompanyTariffByIdAsync(tariff.Id))!;
    }

    public async Task<CompanyTariffDto> UpdateCompanyTariffAsync(int id, UpdateCompanyTariffDto dto)
    {
        var tariff = await _context.CompanyTariffs.FindAsync(id);
        if (tariff == null || tariff.DeletedAt != null)
            throw new NotFoundException(nameof(CompanyTariff), id);

        tariff.CompanyId = dto.CompanyId;
        tariff.RoomTypeId = dto.RoomTypeId;
        tariff.OccupancyCount = dto.OccupancyCount;
        tariff.RatePerNight = dto.RatePerNight;
        tariff.EffectiveFrom = dto.EffectiveFrom;
        tariff.EffectiveTo = dto.EffectiveTo;

        await _context.SaveChangesAsync();

        return (await GetCompanyTariffByIdAsync(id))!;
    }

    public async Task DeleteCompanyTariffAsync(int id)
    {
        var tariff = await _context.CompanyTariffs.FindAsync(id);
        if (tariff == null || tariff.DeletedAt != null)
            throw new NotFoundException(nameof(CompanyTariff), id);

        tariff.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<List<CompanyTariffDto>> CreateBulkCompanyTariffsAsync(int companyId, List<CreateCompanyTariffDto> dtos)
    {
        var tariffs = dtos.Select(dto => new CompanyTariff
        {
            CompanyId = companyId,
            RoomTypeId = dto.RoomTypeId,
            OccupancyCount = dto.OccupancyCount,
            RatePerNight = dto.RatePerNight,
            EffectiveFrom = dto.EffectiveFrom,
            EffectiveTo = dto.EffectiveTo,
            IsActive = true
        }).ToList();

        _context.CompanyTariffs.AddRange(tariffs);
        await _context.SaveChangesAsync();

        return await GetCompanyTariffsAsync(companyId);
    }
}
