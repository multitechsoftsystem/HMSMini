using HMSMini.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMSMini.API.Controllers;

/// <summary>
/// Controller for system settings management
/// Only accessible by Admin role for emergency adjustments
/// </summary>
[ApiController]
[Route("api/system-settings")]
[Authorize(Roles = "Admin")]
public class SystemSettingsController : ControllerBase
{
    private readonly ISystemSettingsService _systemSettingsService;
    private readonly ILogger<SystemSettingsController> _logger;

    public SystemSettingsController(
        ISystemSettingsService systemSettingsService,
        ILogger<SystemSettingsController> logger)
    {
        _systemSettingsService = systemSettingsService;
        _logger = logger;
    }

    /// <summary>
    /// Gets the current working date
    /// </summary>
    [HttpGet("working-date")]
    [ProducesResponseType(typeof(DateTime), StatusCodes.Status200OK)]
    public async Task<ActionResult<DateTime>> GetWorkingDate()
    {
        var workingDate = await _systemSettingsService.GetWorkingDateAsync();
        return Ok(workingDate);
    }

    /// <summary>
    /// Emergency update of working date (Admin only)
    /// WARNING: This bypasses normal day close procedures. Use with extreme caution.
    /// </summary>
    [HttpPut("working-date")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateWorkingDate([FromBody] UpdateWorkingDateRequest request)
    {
        if (request.NewWorkingDate == default)
        {
            return BadRequest("Invalid working date.");
        }

        var currentWorkingDate = await _systemSettingsService.GetWorkingDateAsync();

        if (request.NewWorkingDate < currentWorkingDate.AddDays(-7))
        {
            return BadRequest("Cannot set working date more than 7 days in the past.");
        }

        if (request.NewWorkingDate > DateTime.Today.AddDays(7))
        {
            return BadRequest("Cannot set working date more than 7 days in the future.");
        }

        var username = User.Identity?.Name ?? "System";

        _logger.LogWarning(
            "EMERGENCY: Working date manually updated from {OldDate} to {NewDate} by {User}. " +
            "Reason: {Reason}",
            currentWorkingDate,
            request.NewWorkingDate,
            username,
            request.Reason ?? "Not specified");

        await _systemSettingsService.UpdateWorkingDateAsync(request.NewWorkingDate, username);

        return Ok(new
        {
            message = "Working date updated successfully",
            oldDate = currentWorkingDate,
            newDate = request.NewWorkingDate,
            updatedBy = username,
            warning = "This was an emergency manual update. Normal day close procedures were bypassed."
        });
    }

    /// <summary>
    /// Gets a setting value by key
    /// </summary>
    [HttpGet("{settingKey}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<string>> GetSetting(string settingKey)
    {
        var value = await _systemSettingsService.GetSettingAsync(settingKey);

        if (value == null)
        {
            return NotFound($"Setting '{settingKey}' not found.");
        }

        return Ok(value);
    }

    /// <summary>
    /// Checks if a specific date has been closed
    /// </summary>
    [HttpGet("is-date-closed")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> IsDateClosed([FromQuery] DateTime date)
    {
        var isClosed = await _systemSettingsService.IsDateClosedAsync(date);
        return Ok(isClosed);
    }
}

/// <summary>
/// Request model for updating working date
/// </summary>
public class UpdateWorkingDateRequest
{
    public DateTime NewWorkingDate { get; set; }
    public string? Reason { get; set; }
}
