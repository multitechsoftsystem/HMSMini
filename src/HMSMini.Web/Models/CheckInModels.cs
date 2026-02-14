namespace HMSMini.Web.Models;

public class CheckInDto
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int Pax { get; set; }
    public int Status { get; set; }
    public string StatusName => ((CheckInStatus)Status).ToString();
    public DateTime? ActualCheckInDate { get; set; }
    public DateTime? ActualCheckOutDate { get; set; }
    public string GuestNames { get; set; } = string.Empty;
    public int? CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public string? CompanyGSTNumber { get; set; }
    public int? BusinessSourceId { get; set; }
    public string? BusinessSourceName { get; set; }
    public int? MealPlanId { get; set; }
    public string? MealPlanName { get; set; }
    public int? GuestTypeId { get; set; }
    public string? GuestTypeName { get; set; }
    public decimal? TariffApplied { get; set; }
    public decimal? MealPlanRate { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal? FinalAmount { get; set; }
}

public class CheckInWithGuestsDto : CheckInDto
{
    public List<GuestDto> Guests { get; set; } = new();
}

public class CreateCheckInDto
{
    public string RoomNumber { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; } = DateTime.Today;
    public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(2);
    public DateTime? ActualCheckInDate { get; set; }
    public string? RegistrationNo { get; set; }
    public string? Remarks { get; set; }
    public int? CompanyId { get; set; }
    public int? BusinessSourceId { get; set; }
    public int? MealPlanId { get; set; }
    public int? GuestTypeId { get; set; }
    public List<CreateGuestDto> Guests { get; set; } = new();
}

public class UpdateCheckInModel
{
    public int? CompanyId { get; set; }
    public int? BusinessSourceId { get; set; }
    public int? MealPlanId { get; set; }
    public int? GuestTypeId { get; set; }
    public string? Remarks { get; set; }
}

public enum CheckInStatus
{
    Active = 0,
    CheckedOut = 1,
    Cancelled = 2
}

public class CheckInFinancialData
{
    public int CheckInId { get; set; }
    public decimal TotalAdvanceAmount { get; set; }
    public decimal BillAmount { get; set; }
    public bool IsLoading { get; set; } = true;
    public bool HasError { get; set; } = false;
    public string? ErrorMessage { get; set; }
}
