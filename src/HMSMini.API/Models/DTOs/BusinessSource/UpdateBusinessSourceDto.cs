using System.ComponentModel.DataAnnotations;

namespace HMSMini.API.Models.DTOs.BusinessSource;

/// <summary>
/// DTO for updating an existing business source
/// </summary>
public class UpdateBusinessSourceDto
{
    [Required(ErrorMessage = "Source name is required")]
    [StringLength(100)]
    public string SourceName { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; }
}
