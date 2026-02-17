using HMSMini.API.Models.DTOs.SystemSetting;

namespace HMSMini.API.Services.Interfaces;

/// <summary>
/// Service for managing system-wide settings including working date
/// </summary>
public interface ISystemSettingsService
{
    /// <summary>
    /// Gets all system settings
    /// </summary>
    Task<List<SystemSettingDto>> GetAllSettingsAsync();
    /// <summary>
    /// Gets the current business/working date
    /// </summary>
    Task<DateTime> GetWorkingDateAsync();

    /// <summary>
    /// Updates the working date (called during day close)
    /// </summary>
    /// <param name="newWorkingDate">The new working date to set</param>
    /// <param name="updatedBy">User who is updating the date</param>
    Task UpdateWorkingDateAsync(DateTime newWorkingDate, string? updatedBy = null);

    /// <summary>
    /// Gets a setting value by key
    /// </summary>
    Task<string?> GetSettingAsync(string settingKey);

    /// <summary>
    /// Updates a setting value
    /// </summary>
    Task UpdateSettingAsync(string settingKey, string settingValue, string? updatedBy = null, bool bypassLock = false);

    /// <summary>
    /// Checks if a specific date has been closed
    /// </summary>
    Task<bool> IsDateClosedAsync(DateTime date);

    /// <summary>
    /// Checks if room sharing feature is enabled
    /// </summary>
    Task<bool> IsRoomSharingEnabledAsync();
}
