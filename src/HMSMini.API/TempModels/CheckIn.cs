using System;
using System.Collections.Generic;

namespace HMSMini.API.TempModels;

public partial class CheckIn
{
    public int Id { get; set; }

    public int RoomId { get; set; }

    public DateTime CheckInDate { get; set; }

    public DateTime CheckOutDate { get; set; }

    public DateTime? ActualCheckInDate { get; set; }

    public DateTime? ActualCheckOutDate { get; set; }

    public string? RegistrationNo { get; set; }

    public int Pax { get; set; }

    public int Status { get; set; }

    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletedBy { get; set; }

    public int? CompanyId { get; set; }

    public decimal DiscountPercentage { get; set; }

    public decimal? FinalAmount { get; set; }

    public decimal? TariffApplied { get; set; }

    public int? BusinessSourceId { get; set; }

    public int? MealPlanId { get; set; }

    public decimal? MealPlanRate { get; set; }

    public string? TaxSlabSnapshotJson { get; set; }

    public int TaxType { get; set; }

    public int? GuestTypeId { get; set; }
}
