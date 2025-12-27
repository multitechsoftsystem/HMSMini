using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.CheckIn;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

/// <summary>
/// Manages guest check-ins
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CheckInsController : ControllerBase
{
    private readonly ICheckInService _checkInService;
    private readonly ILogger<CheckInsController> _logger;

    public CheckInsController(ICheckInService checkInService, ILogger<CheckInsController> logger)
    {
        _checkInService = checkInService;
        _logger = logger;
    }

    /// <summary>
    /// Get all check-ins
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<CheckInDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CheckInDto>>> GetAll()
    {
        var checkIns = await _checkInService.GetAllAsync();
        return Ok(checkIns);
    }

    /// <summary>
    /// Get active check-ins
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(List<CheckInDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CheckInDto>>> GetActive()
    {
        var checkIns = await _checkInService.GetActiveCheckInsAsync();
        return Ok(checkIns);
    }

    /// <summary>
    /// Get check-in by ID with guest details
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CheckInWithGuestsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CheckInWithGuestsDto>> GetById(int id)
    {
        var checkIn = await _checkInService.GetByIdAsync(id);
        return Ok(checkIn);
    }

    /// <summary>
    /// Create a new check-in with guests
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager,Receptionist")]
    [ProducesResponseType(typeof(CheckInWithGuestsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CheckInWithGuestsDto>> Create([FromBody] CreateCheckInDto dto)
    {
        var checkIn = await _checkInService.CreateCheckInAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = checkIn.Id }, checkIn);
    }

    /// <summary>
    /// Check out a guest
    /// </summary>
    [HttpPost("{id}/checkout")]
    [Authorize(Roles = "Admin,Manager,Receptionist")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckOut(int id)
    {
        await _checkInService.CheckOutAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Delete a check-in
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _checkInService.DeleteAsync(id);
        return NoContent();
    }
}
