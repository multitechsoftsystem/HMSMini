namespace HMSMini.Web.Models;

public class BaseTariffDto
{
    public int Id { get; set; }
    public int RoomTypeId { get; set; }
    public string RoomTypeName { get; set; } = string.Empty;
    public int OccupancyCount { get; set; }
    public decimal RatePerNight { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; }
}

public class CreateBaseTariffDto
{
    public int RoomTypeId { get; set; }
    public int OccupancyCount { get; set; } = 1;
    public decimal RatePerNight { get; set; }
    public DateTime EffectiveFrom { get; set; } = DateTime.Today;
    public DateTime? EffectiveTo { get; set; }
}

public class UpdateBaseTariffDto
{
    public int RoomTypeId { get; set; }
    public int OccupancyCount { get; set; }
    public decimal RatePerNight { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}

public class CompanyTariffDto
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public int RoomTypeId { get; set; }
    public string RoomTypeName { get; set; } = string.Empty;
    public int OccupancyCount { get; set; }
    public decimal RatePerNight { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; }
}

public class CreateCompanyTariffDto
{
    public int CompanyId { get; set; }
    public int RoomTypeId { get; set; }
    public int OccupancyCount { get; set; } = 1;
    public decimal RatePerNight { get; set; }
    public DateTime EffectiveFrom { get; set; } = DateTime.Today;
    public DateTime? EffectiveTo { get; set; }
}

public class UpdateCompanyTariffDto
{
    public int CompanyId { get; set; }
    public int RoomTypeId { get; set; }
    public int OccupancyCount { get; set; }
    public decimal RatePerNight { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}

public class TariffCalculationDto
{
    public int RoomTypeId { get; set; }
    public string RoomTypeName { get; set; } = string.Empty;
    public int OccupancyCount { get; set; }
    public decimal BaseRate { get; set; }
    public decimal? CompanyRate { get; set; }
    public decimal ApplicableRate { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal FinalRate { get; set; }
    public int NumberOfNights { get; set; }
    public decimal TotalAmount { get; set; }
    public string TariffSource { get; set; } = string.Empty;
}
