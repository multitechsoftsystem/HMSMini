namespace HMSMini.API.Models.DTOs.GuestType;

public class CreateGuestTypeDto
{
    public string TypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
}
