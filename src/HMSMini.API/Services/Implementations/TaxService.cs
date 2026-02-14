using HMSMini.API.Data;
using HMSMini.API.Models.DTOs.Billing;
using HMSMini.API.Models.DTOs.Tax;
using HMSMini.API.Models.Entities;
using HMSMini.API.Models.Enums;
using HMSMini.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HMSMini.API.Services.Implementations;

/// <summary>
/// Service for tax slab-based tax calculations
/// </summary>
public class TaxService : ITaxService
{
    private readonly ApplicationDbContext _context;

    public TaxService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets all active tax slabs for a specific date
    /// </summary>
    public async Task<List<TaxSlabDto>> GetActiveTaxSlabsAsync(DateTime date)
    {
        var slabs = await _context.TaxSlabs
            .Where(ts => ts.IsActive &&
                        ts.EffectiveFrom <= date &&
                        (ts.EffectiveTo == null || ts.EffectiveTo >= date))
            .OrderBy(ts => ts.MinAmount)
            .Select(ts => new TaxSlabDto
            {
                Id = ts.Id,
                MinAmount = ts.MinAmount,
                MaxAmount = ts.MaxAmount,
                CgstPercentage = ts.CgstPercentage,
                SgstPercentage = ts.SgstPercentage,
                IgstPercentage = ts.IgstPercentage,
                EffectiveFrom = ts.EffectiveFrom,
                EffectiveTo = ts.EffectiveTo,
                IsActive = ts.IsActive,
                Description = ts.Description
            })
            .ToListAsync();

        return slabs;
    }

    /// <summary>
    /// Gets the applicable tax slab for a specific daily amount
    /// </summary>
    public async Task<ApplicableTaxSlabDto?> GetApplicableTaxSlabAsync(decimal dailyAmount, DateTime date)
    {
        var slab = await _context.TaxSlabs
            .Where(ts => ts.IsActive &&
                        ts.EffectiveFrom <= date &&
                        (ts.EffectiveTo == null || ts.EffectiveTo >= date) &&
                        ts.MinAmount <= dailyAmount &&
                        (ts.MaxAmount == null || ts.MaxAmount >= dailyAmount))
            .OrderBy(ts => ts.MinAmount)
            .FirstOrDefaultAsync();

        if (slab == null)
            return null;

        var slabRange = slab.MaxAmount.HasValue
            ? $"₹{slab.MinAmount:N2} - ₹{slab.MaxAmount:N2}"
            : $"₹{slab.MinAmount:N2} and above";

        return new ApplicableTaxSlabDto
        {
            Amount = dailyAmount,
            SlabRange = slabRange,
            CgstPercentage = slab.CgstPercentage,
            SgstPercentage = slab.SgstPercentage,
            IgstPercentage = slab.IgstPercentage,
            Description = slab.Description
        };
    }

    /// <summary>
    /// Calculates tax for a given amount using slab-based rates
    /// </summary>
    public async Task<List<TaxLineDto>> CalculateTaxAsync(
        decimal amount,
        TaxType taxType,
        DateTime date,
        TaxSlabSnapshot? snapshot = null)
    {
        // Return empty list if amount is zero
        if (amount <= 0)
            return new List<TaxLineDto>();

        TaxSlabSnapshotItem? applicableSlab;
        string slabRange;

        // Use snapshot if provided (for historical accuracy)
        if (snapshot != null && snapshot.Slabs.Any())
        {
            applicableSlab = snapshot.Slabs
                .Where(s => s.MinAmount <= amount &&
                           (s.MaxAmount == null || s.MaxAmount >= amount))
                .OrderBy(s => s.MinAmount)
                .FirstOrDefault();

            if (applicableSlab == null)
            {
                // No applicable slab found in snapshot, return empty
                return new List<TaxLineDto>();
            }

            slabRange = applicableSlab.MaxAmount.HasValue
                ? $"₹{applicableSlab.MinAmount:N2} - ₹{applicableSlab.MaxAmount:N2}"
                : $"₹{applicableSlab.MinAmount:N2} and above";
        }
        else
        {
            // Fetch live tax slab from database
            var liveSlabDto = await GetApplicableTaxSlabAsync(amount, date);
            if (liveSlabDto == null)
            {
                // No applicable slab found, return empty
                return new List<TaxLineDto>();
            }

            applicableSlab = new TaxSlabSnapshotItem
            {
                MinAmount = 0, // Not needed for calculation
                MaxAmount = null,
                CgstPercentage = liveSlabDto.CgstPercentage,
                SgstPercentage = liveSlabDto.SgstPercentage,
                IgstPercentage = liveSlabDto.IgstPercentage
            };
            slabRange = liveSlabDto.SlabRange;
        }

        var taxLines = new List<TaxLineDto>();

        // Calculate tax based on tax type
        if (taxType == TaxType.Igst)
        {
            var igstAmount = Math.Round((amount * applicableSlab.IgstPercentage) / 100, 2);
            taxLines.Add(new TaxLineDto
            {
                TaxType = "IGST",
                TaxPercentage = applicableSlab.IgstPercentage,
                TaxableAmount = amount,
                TaxAmount = igstAmount,
                SlabRange = slabRange,
                DailyAmount = amount
            });
        }
        else // CGST + SGST
        {
            var cgstAmount = Math.Round((amount * applicableSlab.CgstPercentage) / 100, 2);
            var sgstAmount = Math.Round((amount * applicableSlab.SgstPercentage) / 100, 2);

            taxLines.Add(new TaxLineDto
            {
                TaxType = "CGST",
                TaxPercentage = applicableSlab.CgstPercentage,
                TaxableAmount = amount,
                TaxAmount = cgstAmount,
                SlabRange = slabRange,
                DailyAmount = amount
            });

            taxLines.Add(new TaxLineDto
            {
                TaxType = "SGST",
                TaxPercentage = applicableSlab.SgstPercentage,
                TaxableAmount = amount,
                TaxAmount = sgstAmount,
                SlabRange = slabRange,
                DailyAmount = amount
            });
        }

        return taxLines;
    }

    /// <summary>
    /// Creates a tax slab snapshot at check-in time for historical accuracy
    /// </summary>
    public async Task<TaxSlabSnapshot> CreateTaxSlabSnapshotAsync(DateTime date)
    {
        var slabs = await _context.TaxSlabs
            .Where(ts => ts.IsActive &&
                        ts.EffectiveFrom <= date &&
                        (ts.EffectiveTo == null || ts.EffectiveTo >= date))
            .OrderBy(ts => ts.MinAmount)
            .ToListAsync();

        var snapshot = new TaxSlabSnapshot
        {
            SnapshotDate = date,
            Slabs = slabs.Select(s => new TaxSlabSnapshotItem
            {
                MinAmount = s.MinAmount,
                MaxAmount = s.MaxAmount,
                CgstPercentage = s.CgstPercentage,
                SgstPercentage = s.SgstPercentage,
                IgstPercentage = s.IgstPercentage,
                Description = s.Description
            }).ToList()
        };

        return snapshot;
    }
}
