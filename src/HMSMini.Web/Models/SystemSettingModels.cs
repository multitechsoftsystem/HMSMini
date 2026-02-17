namespace HMSMini.Web.Models;

public class SystemSettingModel
{
    public int Id { get; set; }
    public string SettingKey { get; set; } = string.Empty;
    public string SettingValue { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystemLocked { get; set; }
}
