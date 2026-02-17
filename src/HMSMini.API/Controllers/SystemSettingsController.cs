using HMSMini.API.Models.DTOs.SystemSetting;
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
[Authorize]
public class SystemSettingsController : ControllerBase
{
    private readonly ISystemSettingsService _systemSettingsService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<SystemSettingsController> _logger;

    public SystemSettingsController(
        ISystemSettingsService systemSettingsService,
        IWebHostEnvironment environment,
        ILogger<SystemSettingsController> logger)
    {
        _systemSettingsService = systemSettingsService;
        _environment = environment;
        _logger = logger;
    }

    /// <summary>
    /// Gets all system settings
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Developer")]
    [ProducesResponseType(typeof(List<SystemSettingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<SystemSettingDto>>> GetAllSettings()
    {
        var settings = await _systemSettingsService.GetAllSettingsAsync();
        var isDeveloper = User.IsInRole("Developer");
        if (!isDeveloper)
            settings = settings.Where(s => !s.IsSystemLocked).ToList();
        return Ok(settings);
    }

    /// <summary>
    /// Updates a setting value by key
    /// </summary>
    [HttpPut("{settingKey}")]
    [Authorize(Roles = "Admin,Developer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateSettingByKey(string settingKey, [FromBody] UpdateSettingRequest request)
    {
        try
        {
            var username = User.Identity?.Name ?? "System";
            var isDeveloper = User.IsInRole("Developer");
            await _systemSettingsService.UpdateSettingAsync(settingKey, request.Value, username, bypassLock: isDeveloper);
            return Ok(new { message = $"Setting '{settingKey}' updated successfully" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Setting '{settingKey}' not found.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
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
    [Authorize(Roles = "Admin,Developer")]
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
    /// Uploads a bill heading image and updates the BillHeadingImagePath setting
    /// </summary>
    [HttpPost("bill-heading-image")]
    [Authorize(Roles = "Admin,Developer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UploadBillHeadingImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest("File size must be less than 5 MB.");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            return BadRequest("Invalid file type. Allowed: jpg, jpeg, png, gif, webp.");

        var uploadsDir = Path.Combine(_environment.WebRootPath, "uploads", "settings");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"bill-heading{extension}";
        var filePath = Path.Combine(uploadsDir, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var relativePath = $"/uploads/settings/{fileName}";
        var username = User.Identity?.Name ?? "System";

        // Update the setting - bypass lock since this endpoint is already authorized
        await _systemSettingsService.UpdateSettingAsync("BillHeadingImagePath", relativePath, username, bypassLock: true);

        _logger.LogInformation("Bill heading image uploaded by {User}: {Path}", username, relativePath);

        return Ok(new { path = relativePath });
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

/// <summary>
/// Request model for updating a setting value
/// </summary>
public class UpdateSettingRequest
{
    public string Value { get; set; } = string.Empty;
}
