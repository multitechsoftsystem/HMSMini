using System.ComponentModel.DataAnnotations;

namespace HMSMini.API.Models.DTOs.BusinessSource;

/// <summary>
/// DTO for creating a new business source
/// </summary>
public class CreateBusinessSourceDto
{
    [Required(ErrorMessage = "Source name is required")]
    [StringLength(100)]
    public string SourceName { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}
