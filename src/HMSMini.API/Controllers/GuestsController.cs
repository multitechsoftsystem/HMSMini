using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.Guest;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

/// <summary>
/// Manages guest information
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class GuestsController : ControllerBase
{
    private readonly IGuestService _guestService;
    private readonly ILogger<GuestsController> _logger;

    public GuestsController(IGuestService guestService, ILogger<GuestsController> logger)
    {
        _guestService = guestService;
        _logger = logger;
    }

    /// <summary>
    /// Get guest by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GuestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GuestDto>> GetById(int id)
    {
        var guest = await _guestService.GetByIdAsync(id);
        return Ok(guest);
    }

    /// <summary>
    /// Get all guests for a check-in
    /// </summary>
    [HttpGet("checkin/{checkInId}")]
    [ProducesResponseType(typeof(List<GuestDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GuestDto>>> GetByCheckInId(int checkInId)
    {
        var guests = await _guestService.GetByCheckInIdAsync(checkInId);
        return Ok(guests);
    }

    /// <summary>
    /// Update guest information
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(GuestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GuestDto>> Update(int id, [FromBody] CreateGuestDto dto)
    {
        var guest = await _guestService.UpdateAsync(id, dto);
        return Ok(guest);
    }

    /// <summary>
    /// Delete a guest
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _guestService.DeleteAsync(id);
        return NoContent();
    }
}
