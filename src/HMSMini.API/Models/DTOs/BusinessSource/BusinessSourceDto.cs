namespace HMSMini.API.Models.DTOs.BusinessSource;

/// <summary>
/// DTO for displaying business source information
/// </summary>
public class BusinessSourceDto
{
    public int BusinessSourceId { get; set; }
    public string SourceName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
