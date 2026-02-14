using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.GuestType;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GuestTypesController : ControllerBase
{
    private readonly IGuestTypeService _guestTypeService;
    private readonly ILogger<GuestTypesController> _logger;

    public GuestTypesController(IGuestTypeService guestTypeService, ILogger<GuestTypesController> logger)
    {
        _guestTypeService = guestTypeService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<GuestTypeDto>>> GetAll([FromQuery] bool includeInactive = false)
    {
        var guestTypes = await _guestTypeService.GetAllAsync(includeInactive);
        return Ok(guestTypes);
    }

    [HttpGet("active")]
    public async Task<ActionResult<List<GuestTypeDto>>> GetActive()
    {
        var guestTypes = await _guestTypeService.GetActiveAsync();
        return Ok(guestTypes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GuestTypeDto>> GetById(int id)
    {
        var guestType = await _guestTypeService.GetByIdAsync(id);
        if (guestType == null)
            return NotFound();

        return Ok(guestType);
    }

    [HttpPost]
    public async Task<ActionResult<GuestTypeDto>> Create([FromBody] CreateGuestTypeDto dto)
    {
        var guestType = await _guestTypeService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = guestType.GuestTypeId }, guestType);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<GuestTypeDto>> Update(int id, [FromBody] UpdateGuestTypeDto dto)
    {
        var guestType = await _guestTypeService.UpdateAsync(id, dto);
        return Ok(guestType);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _guestTypeService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/activate")]
    public async Task<ActionResult<GuestTypeDto>> Activate(int id)
    {
        var guestType = await _guestTypeService.ActivateAsync(id);
        return Ok(guestType);
    }

    [HttpPost("{id}/deactivate")]
    public async Task<ActionResult<GuestTypeDto>> Deactivate(int id)
    {
        var guestType = await _guestTypeService.DeactivateAsync(id);
        return Ok(guestType);
    }
}
