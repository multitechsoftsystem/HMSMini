using HMSMini.API.Models.DTOs.Billing;
using HMSMini.API.Models.DTOs.Tax;
using HMSMini.API.Models.Entities;
using HMSMini.API.Models.Enums;

namespace HMSMini.API.Services.Interfaces;

/// <summary>
/// Service for tax slab-based tax calculations
/// </summary>
public interface ITaxService
{
    /// <summary>
    /// Gets all active tax slabs for a specific date
    /// </summary>
    Task<List<TaxSlabDto>> GetActiveTaxSlabsAsync(DateTime date);

    /// <summary>
    /// Gets the applicable tax slab for a specific daily amount
    /// </summary>
    Task<ApplicableTaxSlabDto?> GetApplicableTaxSlabAsync(decimal dailyAmount, DateTime date);

    /// <summary>
    /// Calculates tax for a given amount using slab-based rates
    /// </summary>
    /// <param name="amount">Amount to calculate tax on</param>
    /// <param name="taxType">IGST or CGST+SGST</param>
    /// <param name="date">Date for determining applicable tax slab</param>
    /// <param name="snapshot">Optional tax snapshot from check-in time</param>
    Task<List<TaxLineDto>> CalculateTaxAsync(
        decimal amount,
        TaxType taxType,
        DateTime date,
        TaxSlabSnapshot? snapshot = null);

    /// <summary>
    /// Creates a tax slab snapshot at check-in time for historical accuracy
    /// </summary>
    Task<TaxSlabSnapshot> CreateTaxSlabSnapshotAsync(DateTime date);
}
