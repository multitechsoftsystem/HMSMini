namespace HMSMini.API.Models.Entities;

/// <summary>
/// System-wide settings including the working date for daily operations
/// </summary>
public class SystemSetting
{
    public int Id { get; set; }

    /// <summary>
    /// Unique key for the setting (e.g., "WorkingDate")
    /// </summary>
    public string SettingKey { get; set; } = string.Empty;

    /// <summary>
    /// String representation of the setting value
    /// </summary>
    public string SettingValue { get; set; } = string.Empty;

    /// <summary>
    /// Data type of the setting (Date, String, Int, Bool, etc.)
    /// </summary>
    public string DataType { get; set; } = string.Empty;

    /// <summary>
    /// Description of what this setting controls
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// If true, only system administrators can modify this setting
    /// </summary>
    public bool IsSystemLocked { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
