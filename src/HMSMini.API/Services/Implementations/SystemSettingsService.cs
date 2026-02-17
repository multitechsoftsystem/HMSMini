using HMSMini.API.Data;
using HMSMini.API.Models.DTOs.SystemSetting;
using HMSMini.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HMSMini.API.Services.Implementations;

/// <summary>
/// Service for managing system-wide settings
/// </summary>
public class SystemSettingsService : ISystemSettingsService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SystemSettingsService> _logger;

    public SystemSettingsService(
        ApplicationDbContext context,
        ILogger<SystemSettingsService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<SystemSettingDto>> GetAllSettingsAsync()
    {
        return await _context.SystemSettings
            .AsNoTracking()
            .Select(s => new SystemSettingDto
            {
                Id = s.Id,
                SettingKey = s.SettingKey,
                SettingValue = s.SettingValue,
                DataType = s.DataType,
                Description = s.Description,
                IsSystemLocked = s.IsSystemLocked
            })
            .ToListAsync();
    }

    /// <summary>
    /// Gets the current working date from SystemSettings
    /// </summary>
    public async Task<DateTime> GetWorkingDateAsync()
    {
        var setting = await _context.SystemSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SettingKey == "WorkingDate");

        if (setting == null)
        {
            _logger.LogWarning("WorkingDate setting not found. Returning today's date.");
            return DateTime.Today;
        }

        if (DateTime.TryParse(setting.SettingValue, out DateTime workingDate))
        {
            return workingDate.Date;
        }

        _logger.LogError("Invalid WorkingDate value: {Value}. Returning today's date.", setting.SettingValue);
        return DateTime.Today;
    }

    /// <summary>
    /// Updates the working date (typically called during day close)
    /// </summary>
    public async Task UpdateWorkingDateAsync(DateTime newWorkingDate, string? updatedBy = null)
    {
        var setting = await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.SettingKey == "WorkingDate");

        if (setting == null)
        {
            throw new InvalidOperationException("WorkingDate setting not found in SystemSettings.");
        }

        var oldValue = setting.SettingValue;
        setting.SettingValue = newWorkingDate.Date.ToString("yyyy-MM-dd");
        setting.UpdatedAt = DateTime.UtcNow;
        setting.UpdatedBy = updatedBy;

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Working date updated from {OldDate} to {NewDate} by {User}",
            oldValue,
            setting.SettingValue,
            updatedBy ?? "System");
    }

    /// <summary>
    /// Gets a setting value by key
    /// </summary>
    public async Task<string?> GetSettingAsync(string settingKey)
    {
        var setting = await _context.SystemSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SettingKey == settingKey);

        return setting?.SettingValue;
    }

    /// <summary>
    /// Updates a setting value
    /// </summary>
    public async Task UpdateSettingAsync(string settingKey, string settingValue, string? updatedBy = null, bool bypassLock = false)
    {
        var setting = await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.SettingKey == settingKey);

        if (setting == null)
        {
            throw new KeyNotFoundException($"Setting '{settingKey}' not found.");
        }

        if (setting.IsSystemLocked && !bypassLock)
        {
            throw new InvalidOperationException($"Setting '{settingKey}' is system-locked and cannot be modified.");
        }

        setting.SettingValue = settingValue;
        setting.UpdatedAt = DateTime.UtcNow;
        setting.UpdatedBy = updatedBy;

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Setting '{Key}' updated to '{Value}' by {User}",
            settingKey,
            settingValue,
            updatedBy ?? "System");
    }

    /// <summary>
    /// Checks if a specific date has been closed
    /// A date is considered closed if it's before the current working date
    /// </summary>
    public async Task<bool> IsDateClosedAsync(DateTime date)
    {
        var workingDate = await GetWorkingDateAsync();
        return date.Date < workingDate.Date;
    }

    /// <summary>
    /// Checks if the room sharing feature is enabled
    /// </summary>
    public async Task<bool> IsRoomSharingEnabledAsync()
    {
        var value = await GetSettingAsync("EnableRoomSharing");
        return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
    }
}
