using System.ComponentModel.DataAnnotations;

namespace HMSMini.API.Models.DTOs.EventType;

public class UpdateEventTypeDto
{
    [Required(ErrorMessage = "Event type name is required")]
    [StringLength(200)]
    public string EventTypeName { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}
