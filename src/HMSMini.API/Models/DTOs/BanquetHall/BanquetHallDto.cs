namespace HMSMini.API.Models.DTOs.BanquetHall;

public class BanquetHallDto
{
    public int Id { get; set; }
    public string HallName { get; set; } = string.Empty;
    public int MaxCapacity { get; set; }
    public int MinCapacity { get; set; }
    public decimal RentPerEvent { get; set; }
    public string? Location { get; set; }
    public string? Features { get; set; }
    public string? ImagePath { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
