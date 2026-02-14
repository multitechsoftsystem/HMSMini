using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.BusinessSource;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

/// <summary>
/// Controller for managing business sources/booking channels
/// </summary>
[ApiController]
[Route("api/business-sources")]
[Authorize]
public class BusinessSourcesController : ControllerBase
{
    private readonly IBusinessSourceService _businessSourceService;
    private readonly ILogger<BusinessSourcesController> _logger;

    public BusinessSourcesController(
        IBusinessSourceService businessSourceService,
        ILogger<BusinessSourcesController> logger)
    {
        _businessSourceService = businessSourceService;
        _logger = logger;
    }

    /// <summary>
    /// Get all business sources
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<BusinessSourceDto>>> GetAll([FromQuery] bool includeInactive = false)
    {
        var businessSources = await _businessSourceService.GetAllAsync(includeInactive);
        return Ok(businessSources);
    }

    /// <summary>
    /// Get active business sources (for dropdown)
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<List<BusinessSourceDto>>> GetActiveBusinessSources()
    {
        var businessSources = await _businessSourceService.GetActiveBusinessSourcesAsync();
        return Ok(businessSources);
    }

    /// <summary>
    /// Get business source by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<BusinessSourceDto>> GetById(int id)
    {
        var businessSource = await _businessSourceService.GetByIdAsync(id);
        if (businessSource == null)
        {
            return NotFound($"Business source with ID {id} not found.");
        }
        return Ok(businessSource);
    }

    /// <summary>
    /// Create a new business source
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<BusinessSourceDto>> Create([FromBody] CreateBusinessSourceDto dto)
    {
        var businessSource = await _businessSourceService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = businessSource.BusinessSourceId }, businessSource);
    }

    /// <summary>
    /// Update a business source
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<BusinessSourceDto>> Update(int id, [FromBody] UpdateBusinessSourceDto dto)
    {
        var businessSource = await _businessSourceService.UpdateAsync(id, dto);
        return Ok(businessSource);
    }

    /// <summary>
    /// Delete a business source (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        await _businessSourceService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Activate a business source
    /// </summary>
    [HttpPost("{id}/activate")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<BusinessSourceDto>> Activate(int id)
    {
        var businessSource = await _businessSourceService.ActivateAsync(id);
        return Ok(businessSource);
    }

    /// <summary>
    /// Deactivate a business source
    /// </summary>
    [HttpPost("{id}/deactivate")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<BusinessSourceDto>> Deactivate(int id)
    {
        var businessSource = await _businessSourceService.DeactivateAsync(id);
        return Ok(businessSource);
    }
}
