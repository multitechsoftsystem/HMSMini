using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.BanquetHall;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

[ApiController]
[Route("api/banquet-halls")]
[Authorize]
public class BanquetHallsController : ControllerBase
{
    private readonly IBanquetHallService _hallService;
    private readonly ILogger<BanquetHallsController> _logger;

    public BanquetHallsController(IBanquetHallService hallService, ILogger<BanquetHallsController> logger)
    {
        _hallService = hallService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<BanquetHallDto>>> GetAll([FromQuery] bool includeInactive = false)
    {
        var halls = await _hallService.GetAllAsync(includeInactive);
        return Ok(halls);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BanquetHallDto>> GetById(int id)
    {
        var hall = await _hallService.GetByIdAsync(id);
        if (hall == null) return NotFound($"Banquet hall with ID {id} not found.");
        return Ok(hall);
    }

    [HttpGet("{id}/availability")]
    public async Task<ActionResult<object>> CheckAvailability(int id, [FromQuery] DateTime date, [FromQuery] TimeSpan startTime, [FromQuery] TimeSpan endTime)
    {
        var isAvailable = await _hallService.CheckAvailabilityAsync(id, date, startTime, endTime);
        return Ok(new { hallId = id, date, startTime, endTime, isAvailable });
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager,BanquetManager")]
    public async Task<ActionResult<BanquetHallDto>> Create([FromBody] CreateBanquetHallDto dto)
    {
        var hall = await _hallService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = hall.Id }, hall);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager,BanquetManager")]
    public async Task<ActionResult<BanquetHallDto>> Update(int id, [FromBody] UpdateBanquetHallDto dto)
    {
        var hall = await _hallService.UpdateAsync(id, dto);
        return Ok(hall);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        await _hallService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/activate")]
    [Authorize(Roles = "Admin,Manager,BanquetManager")]
    public async Task<ActionResult<BanquetHallDto>> Activate(int id)
    {
        var hall = await _hallService.ActivateAsync(id);
        return Ok(hall);
    }

    [HttpPost("{id}/deactivate")]
    [Authorize(Roles = "Admin,Manager,BanquetManager")]
    public async Task<ActionResult<BanquetHallDto>> Deactivate(int id)
    {
        var hall = await _hallService.DeactivateAsync(id);
        return Ok(hall);
    }
}
