namespace HMSMini.API.Models.Entities;

/// <summary>
/// Master table for guest types (Normal, Complimentary, Family, Hotel2, Hotel3, etc.)
/// </summary>
public class MGuestType
{
    public int GuestTypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public virtual ICollection<CheckIn> CheckIns { get; set; } = new List<CheckIn>();
}
