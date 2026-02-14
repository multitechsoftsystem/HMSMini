using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.EventType;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

[ApiController]
[Route("api/event-types")]
[Authorize]
public class EventTypesController : ControllerBase
{
    private readonly IEventTypeService _eventTypeService;
    private readonly ILogger<EventTypesController> _logger;

    public EventTypesController(IEventTypeService eventTypeService, ILogger<EventTypesController> logger)
    {
        _eventTypeService = eventTypeService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<EventTypeDto>>> GetAll([FromQuery] bool includeInactive = false)
    {
        var types = await _eventTypeService.GetAllAsync(includeInactive);
        return Ok(types);
    }

    [HttpGet("active")]
    public async Task<ActionResult<List<EventTypeDto>>> GetActive()
    {
        var types = await _eventTypeService.GetActiveAsync();
        return Ok(types);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EventTypeDto>> GetById(int id)
    {
        var type = await _eventTypeService.GetByIdAsync(id);
        if (type == null) return NotFound($"Event type with ID {id} not found.");
        return Ok(type);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager,BanquetManager")]
    public async Task<ActionResult<EventTypeDto>> Create([FromBody] CreateEventTypeDto dto)
    {
        var type = await _eventTypeService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = type.Id }, type);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager,BanquetManager")]
    public async Task<ActionResult<EventTypeDto>> Update(int id, [FromBody] UpdateEventTypeDto dto)
    {
        var type = await _eventTypeService.UpdateAsync(id, dto);
        return Ok(type);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        await _eventTypeService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/activate")]
    [Authorize(Roles = "Admin,Manager,BanquetManager")]
    public async Task<ActionResult<EventTypeDto>> Activate(int id)
    {
        var type = await _eventTypeService.ActivateAsync(id);
        return Ok(type);
    }

    [HttpPost("{id}/deactivate")]
    [Authorize(Roles = "Admin,Manager,BanquetManager")]
    public async Task<ActionResult<EventTypeDto>> Deactivate(int id)
    {
        var type = await _eventTypeService.DeactivateAsync(id);
        return Ok(type);
    }
}
