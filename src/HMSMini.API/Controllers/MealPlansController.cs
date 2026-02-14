using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.MealPlan;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

/// <summary>
/// Controller for managing meal plans
/// </summary>
[ApiController]
[Route("api/meal-plans")]
[Authorize]
public class MealPlansController : ControllerBase
{
    private readonly IMealPlanService _mealPlanService;
    private readonly ILogger<MealPlansController> _logger;

    public MealPlansController(
        IMealPlanService mealPlanService,
        ILogger<MealPlansController> logger)
    {
        _mealPlanService = mealPlanService;
        _logger = logger;
    }

    /// <summary>
    /// Get all meal plans
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<MealPlanDto>>> GetAll([FromQuery] bool includeInactive = false)
    {
        var mealPlans = await _mealPlanService.GetAllMealPlansAsync(includeInactive);
        return Ok(mealPlans);
    }

    /// <summary>
    /// Get active meal plans (for dropdown)
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<List<MealPlanDto>>> GetActiveMealPlans()
    {
        var mealPlans = await _mealPlanService.GetActiveMealPlansAsync();
        return Ok(mealPlans);
    }

    /// <summary>
    /// Get meal plan by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<MealPlanDto>> GetById(int id)
    {
        var mealPlan = await _mealPlanService.GetMealPlanByIdAsync(id);
        if (mealPlan == null)
        {
            return NotFound($"Meal plan with ID {id} not found.");
        }
        return Ok(mealPlan);
    }

    /// <summary>
    /// Create a new meal plan
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<MealPlanDto>> Create([FromBody] CreateMealPlanDto dto)
    {
        var mealPlan = await _mealPlanService.CreateMealPlanAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = mealPlan.MealPlanId }, mealPlan);
    }

    /// <summary>
    /// Update a meal plan
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<MealPlanDto>> Update(int id, [FromBody] UpdateMealPlanDto dto)
    {
        var mealPlan = await _mealPlanService.UpdateMealPlanAsync(id, dto);
        return Ok(mealPlan);
    }

    /// <summary>
    /// Delete a meal plan (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        await _mealPlanService.DeleteMealPlanAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Activate a meal plan
    /// </summary>
    [HttpPost("{id}/activate")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<MealPlanDto>> Activate(int id)
    {
        var mealPlan = await _mealPlanService.ActivateMealPlanAsync(id);
        return Ok(mealPlan);
    }

    /// <summary>
    /// Deactivate a meal plan
    /// </summary>
    [HttpPost("{id}/deactivate")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<MealPlanDto>> Deactivate(int id)
    {
        var mealPlan = await _mealPlanService.DeactivateMealPlanAsync(id);
        return Ok(mealPlan);
    }
}
