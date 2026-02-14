namespace HMSMini.Web.Models;

public class GuestTypeModel
{
    public int GuestTypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateGuestTypeModel
{
    public string TypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
}

public class UpdateGuestTypeModel
{
    public string? TypeName { get; set; }
    public string? Description { get; set; }
    public int? DisplayOrder { get; set; }
}
