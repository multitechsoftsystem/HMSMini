using System.ComponentModel.DataAnnotations;
using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.DTOs.CheckIn;

/// <summary>
/// DTO for updating check-in tax type
/// </summary>
public class UpdateCheckInTaxTypeDto
{
    [Required]
    public TaxType TaxType { get; set; }
}
