using System.ComponentModel.DataAnnotations;
using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.DTOs.Tax;

/// <summary>
/// Request DTO for tax calculation
/// </summary>
public class TaxCalculationRequest
{
    [Required]
    [Range(0.01, 999999999.99)]
    public decimal Amount { get; set; }

    [Required]
    public TaxType TaxType { get; set; }

    public DateTime? Date { get; set; }
}
