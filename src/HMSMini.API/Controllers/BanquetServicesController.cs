using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.BanquetService;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

[ApiController]
[Route("api/banquet-services")]
[Authorize]
public class BanquetServicesController : ControllerBase
{
    private readonly IBanquetServiceService _banquetServiceService;
    private readonly ILogger<BanquetServicesController> _logger;

    public BanquetServicesController(IBanquetServiceService banquetServiceService, ILogger<BanquetServicesController> logger)
    {
        _banquetServiceService = banquetServiceService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<BanquetServiceDto>>> GetAll([FromQuery] bool includeInactive = false)
    {
        var services = await _banquetServiceService.GetAllAsync(includeInactive);
        return Ok(services);
    }

    [HttpGet("active")]
    public async Task<ActionResult<List<BanquetServiceDto>>> GetActive()
    {
        var services = await _banquetServiceService.GetActiveAsync();
        return Ok(services);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BanquetServiceDto>> GetById(int id)
    {
        var service = await _banquetServiceService.GetByIdAsync(id);
        if (service == null) return NotFound($"Banquet service with ID {id} not found.");
        return Ok(service);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager,BanquetManager")]
    public async Task<ActionResult<BanquetServiceDto>> Create([FromBody] CreateBanquetServiceDto dto)
    {
        var service = await _banquetServiceService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = service.Id }, service);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager,BanquetManager")]
    public async Task<ActionResult<BanquetServiceDto>> Update(int id, [FromBody] UpdateBanquetServiceDto dto)
    {
        var service = await _banquetServiceService.UpdateAsync(id, dto);
        return Ok(service);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        await _banquetServiceService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/activate")]
    [Authorize(Roles = "Admin,Manager,BanquetManager")]
    public async Task<ActionResult<BanquetServiceDto>> Activate(int id)
    {
        var service = await _banquetServiceService.ActivateAsync(id);
        return Ok(service);
    }

    [HttpPost("{id}/deactivate")]
    [Authorize(Roles = "Admin,Manager,BanquetManager")]
    public async Task<ActionResult<BanquetServiceDto>> Deactivate(int id)
    {
        var service = await _banquetServiceService.DeactivateAsync(id);
        return Ok(service);
    }
}
