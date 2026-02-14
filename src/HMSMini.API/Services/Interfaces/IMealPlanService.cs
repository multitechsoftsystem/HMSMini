using HMSMini.API.Models.DTOs.MealPlan;

namespace HMSMini.API.Services.Interfaces;

/// <summary>
/// Service interface for meal plan operations
/// </summary>
public interface IMealPlanService
{
    // Meal Plan operations
    Task<List<MealPlanDto>> GetAllMealPlansAsync(bool includeInactive = false);
    Task<List<MealPlanDto>> GetActiveMealPlansAsync();
    Task<MealPlanDto?> GetMealPlanByIdAsync(int id);
    Task<MealPlanDto> CreateMealPlanAsync(CreateMealPlanDto dto);
    Task<MealPlanDto> UpdateMealPlanAsync(int id, UpdateMealPlanDto dto);
    Task DeleteMealPlanAsync(int id);
    Task<MealPlanDto> ActivateMealPlanAsync(int id);
    Task<MealPlanDto> DeactivateMealPlanAsync(int id);

    // Meal Plan Rate operations
    Task<List<MealPlanRateDto>> GetAllMealPlanRatesAsync(bool includeInactive = false);
    Task<List<MealPlanRateDto>> GetMealPlanRatesByMealPlanIdAsync(int mealPlanId);
    Task<List<MealPlanRateDto>> GetMealPlanRatesByRoomTypeIdAsync(int roomTypeId);
    Task<MealPlanRateDto?> GetMealPlanRateByIdAsync(int id);
    Task<MealPlanRateDto> CreateMealPlanRateAsync(CreateMealPlanRateDto dto);
    Task<MealPlanRateDto> UpdateMealPlanRateAsync(int id, UpdateMealPlanRateDto dto);
    Task DeleteMealPlanRateAsync(int id);

    // Utility methods
    Task<decimal?> GetMealPlanRateForBookingAsync(int mealPlanId, int roomTypeId, DateTime checkInDate);
}
