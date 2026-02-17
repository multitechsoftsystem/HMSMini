namespace HMSMini.API.Models.DTOs.SystemSetting;

public class SystemSettingDto
{
    public int Id { get; set; }
    public string SettingKey { get; set; } = string.Empty;
    public string SettingValue { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystemLocked { get; set; }
}
