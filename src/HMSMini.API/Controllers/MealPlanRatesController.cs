using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.MealPlan;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

/// <summary>
/// Controller for managing meal plan rates
/// </summary>
[ApiController]
[Route("api/meal-plan-rates")]
[Authorize]
public class MealPlanRatesController : ControllerBase
{
    private readonly IMealPlanService _mealPlanService;
    private readonly ILogger<MealPlanRatesController> _logger;

    public MealPlanRatesController(
        IMealPlanService mealPlanService,
        ILogger<MealPlanRatesController> logger)
    {
        _mealPlanService = mealPlanService;
        _logger = logger;
    }

    /// <summary>
    /// Get all meal plan rates
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<MealPlanRateDto>>> GetAll([FromQuery] bool includeInactive = false)
    {
        var rates = await _mealPlanService.GetAllMealPlanRatesAsync(includeInactive);
        return Ok(rates);
    }

    /// <summary>
    /// Get meal plan rates by meal plan ID
    /// </summary>
    [HttpGet("by-meal-plan/{mealPlanId}")]
    public async Task<ActionResult<List<MealPlanRateDto>>> GetByMealPlanId(int mealPlanId)
    {
        var rates = await _mealPlanService.GetMealPlanRatesByMealPlanIdAsync(mealPlanId);
        return Ok(rates);
    }

    /// <summary>
    /// Get meal plan rates by room type ID
    /// </summary>
    [HttpGet("by-room-type/{roomTypeId}")]
    public async Task<ActionResult<List<MealPlanRateDto>>> GetByRoomTypeId(int roomTypeId)
    {
        var rates = await _mealPlanService.GetMealPlanRatesByRoomTypeIdAsync(roomTypeId);
        return Ok(rates);
    }

    /// <summary>
    /// Get meal plan rate by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<MealPlanRateDto>> GetById(int id)
    {
        var rate = await _mealPlanService.GetMealPlanRateByIdAsync(id);
        if (rate == null)
        {
            return NotFound($"Meal plan rate with ID {id} not found.");
        }
        return Ok(rate);
    }

    /// <summary>
    /// Create a new meal plan rate
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<MealPlanRateDto>> Create([FromBody] CreateMealPlanRateDto dto)
    {
        var rate = await _mealPlanService.CreateMealPlanRateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = rate.Id }, rate);
    }

    /// <summary>
    /// Update a meal plan rate
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<MealPlanRateDto>> Update(int id, [FromBody] UpdateMealPlanRateDto dto)
    {
        var rate = await _mealPlanService.UpdateMealPlanRateAsync(id, dto);
        return Ok(rate);
    }

    /// <summary>
    /// Delete a meal plan rate (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        await _mealPlanService.DeleteMealPlanRateAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Get meal plan rate for a specific booking scenario
    /// </summary>
    [HttpGet("for-booking")]
    public async Task<ActionResult<decimal?>> GetRateForBooking(
        [FromQuery] int mealPlanId,
        [FromQuery] int roomTypeId,
        [FromQuery] DateTime checkInDate)
    {
        var rate = await _mealPlanService.GetMealPlanRateForBookingAsync(mealPlanId, roomTypeId, checkInDate);
        if (rate == null)
        {
            return NotFound($"No meal plan rate found for the specified parameters.");
        }
        return Ok(rate);
    }
}
