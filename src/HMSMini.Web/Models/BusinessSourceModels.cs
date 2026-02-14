namespace HMSMini.Web.Models;

public class BusinessSourceModel
{
    public int BusinessSourceId { get; set; }
    public string SourceName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateBusinessSourceModel
{
    public string SourceName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateBusinessSourceModel
{
    public string? SourceName { get; set; }
    public string? Description { get; set; }
}
