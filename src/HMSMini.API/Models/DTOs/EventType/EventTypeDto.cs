namespace HMSMini.API.Models.DTOs.EventType;

public class EventTypeDto
{
    public int Id { get; set; }
    public string EventTypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
