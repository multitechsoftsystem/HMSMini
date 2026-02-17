using System.Text.Json;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.Billing;
using HMSMini.API.Models.Entities;
using HMSMini.API.Models.Enums;
using HMSMini.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMSMini.API.Services.Implementations;

public class BillingService : IBillingService
{
    private readonly ApplicationDbContext _context;
    private readonly ITaxService _taxService;
    private readonly ISystemSettingsService _systemSettingsService;
    private readonly ILogger<BillingService> _logger;

    public BillingService(
        ApplicationDbContext context,
        ITaxService taxService,
        ISystemSettingsService systemSettingsService,
        ILogger<BillingService> logger)
    {
        _context = context;
        _taxService = taxService;
        _systemSettingsService = systemSettingsService;
        _logger = logger;
    }

    public async Task<BillPreviewDto> GetBillPreviewAsync(int checkInId, DateTime? proposedCheckOutDate = null)
    {
        // Retrieve check-in with all necessary relationships
        var checkIn = await _context.CheckIns
            .Include(c => c.Room)
                .ThenInclude(r => r.RoomType)
            .Include(c => c.Company)
            .FirstOrDefaultAsync(c => c.Id == checkInId && c.DeletedAt == null);

        if (checkIn == null)
            throw new NotFoundException(nameof(CheckIn), checkInId);

        if (checkIn.Status != CheckInStatus.Active)
            throw new BusinessRuleException("Cannot generate bill preview for non-active check-in");

        // Determine dates
        var actualCheckInDate = checkIn.ActualCheckInDate ?? checkIn.CheckInDate;
        var checkOutDate = proposedCheckOutDate ?? checkIn.ActualCheckOutDate ?? checkIn.CheckOutDate;

        // Get working date and cap checkout to only show posted charges
        var workingDate = await _systemSettingsService.GetWorkingDateAsync();

        // Only show charges up to (but not including) working date
        // If guest already checked out before working date, use actual checkout
        var effectiveCheckOutDate = checkOutDate > workingDate
            ? workingDate
            : checkOutDate;

        _logger.LogInformation(
            "Bill preview for CheckIn {CheckInId}: OriginalCheckout={OriginalCheckout}, " +
            "WorkingDate={WorkingDate}, EffectiveCheckout={EffectiveCheckout}",
            checkInId,
            checkOutDate,
            workingDate,
            effectiveCheckOutDate);

        // Get guest count
        var pax = await _context.Guests
            .CountAsync(g => g.CheckInId == checkInId && g.DeletedAt == null);

        // Get guest names
        var guestNames = string.Join(", ", await _context.Guests
            .Where(g => g.CheckInId == checkInId && g.DeletedAt == null)
            .OrderBy(g => g.GuestNumber)
            .Select(g => g.GuestName)
            .ToListAsync());

        // Parse tax snapshot for historical accuracy
        TaxSlabSnapshot? taxSnapshot = null;
        if (!string.IsNullOrEmpty(checkIn.TaxSlabSnapshotJson))
        {
            try
            {
                taxSnapshot = JsonSerializer.Deserialize<TaxSlabSnapshot>(checkIn.TaxSlabSnapshotJson);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize tax snapshot for check-in {CheckInId}", checkInId);
            }
        }

        // Calculate day-by-day charges with tax type and snapshot
        // Use stored TariffApplied and MealPlanRate as fallbacks for older check-ins
        // Use effectiveCheckOutDate to only show charges up to working date
        var dailyCharges = await CalculateDailyChargesAsync(
            checkIn.Room.RoomTypeId,
            pax,
            actualCheckInDate,
            effectiveCheckOutDate,
            checkIn.CompanyId,
            checkIn.MealPlanId,
            checkIn.DiscountPercentage,
            checkIn.TaxType,
            taxSnapshot,
            checkIn.TariffApplied ?? 0,
            checkIn.MealPlanRate ?? 0);

        // Get additional charges entities (with VoucherTaxConfig relationship)
        var additionalChargeEntities = await _context.AdditionalCharges
            .Include(a => a.VoucherTaxConfig)
            .Where(a => a.CheckInId == checkInId && a.DeletedAt == null)
            .ToListAsync();

        // Get additional charges DTOs for display
        var additionalCharges = await GetChargesByCheckInAsync(checkInId);

        // Calculate totals
        var roomChargesSubtotal = dailyCharges.Sum(d => d.RoomRate);
        var mealChargesSubtotal = dailyCharges.Sum(d => d.MealPlanRate);
        var discountAmount = dailyCharges.Sum(d => d.DiscountAmount);
        var additionalChargesSubtotal = additionalCharges.Sum(a => a.TotalAmount);

        // Calculate tax on additional charges using voucher-specific rates
        var additionalChargesTax = await CalculateTaxForAdditionalChargesAsync(
            additionalChargeEntities,
            checkIn.TaxType,
            actualCheckInDate,
            taxSnapshot);
        var additionalChargesTaxAmount = additionalChargesTax.Sum(t => t.TaxAmount);

        var subtotalBeforeTax = dailyCharges.Sum(d => d.SubtotalAfterDiscount) + additionalChargesSubtotal;
        var totalTax = dailyCharges.Sum(d => d.TotalTax) + additionalChargesTaxAmount;
        var grandTotal = dailyCharges.Sum(d => d.DayTotal) + additionalChargesSubtotal + additionalChargesTaxAmount;

        // Aggregate tax breakdown
        var taxBreakdown = AggregateTaxBreakdown(dailyCharges, additionalChargesTax);

        return new BillPreviewDto
        {
            CheckInId = checkInId,
            RoomNumber = checkIn.Room.RoomNumber,
            GuestNames = guestNames,
            CompanyName = checkIn.Company?.CompanyName,
            ActualCheckInDate = actualCheckInDate,
            ProposedCheckOutDate = checkOutDate,
            TotalNights = dailyCharges.Count,
            Pax = pax,
            DailyCharges = dailyCharges,
            AdditionalCharges = additionalCharges,
            RoomChargesSubtotal = roomChargesSubtotal,
            MealChargesSubtotal = mealChargesSubtotal,
            DiscountAmount = discountAmount,
            AdditionalChargesSubtotal = additionalChargesSubtotal,
            SubtotalBeforeTax = subtotalBeforeTax,
            TaxBreakdown = taxBreakdown,
            TotalTax = totalTax,
            GrandTotal = grandTotal,
            GeneratedAt = DateTime.UtcNow
        };
    }

    public async Task<InvoiceDto> FinalizeInvoiceAsync(int checkInId, FinalizeInvoiceDto dto)
    {
        // Retrieve check-in
        var checkIn = await _context.CheckIns
            .Include(c => c.Room)
                .ThenInclude(r => r.RoomType)
            .Include(c => c.Company)
            .FirstOrDefaultAsync(c => c.Id == checkInId && c.DeletedAt == null);

        if (checkIn == null)
            throw new NotFoundException(nameof(CheckIn), checkInId);

        if (checkIn.Status != CheckInStatus.Active)
            throw new BusinessRuleException("Cannot finalize invoice for non-active check-in");

        // Check if invoice already exists
        var existingInvoice = await _context.Invoices
            .FirstOrDefaultAsync(i => i.CheckInId == checkInId && i.DeletedAt == null);

        if (existingInvoice != null)
            throw new BusinessRuleException("Invoice already exists for this check-in");

        // Set actual checkout date
        var actualCheckOutDate = dto.ActualCheckOutDate ?? DateTime.UtcNow;
        checkIn.ActualCheckOutDate = actualCheckOutDate;

        // Generate bill preview
        var billPreview = await GetBillPreviewAsync(checkInId, actualCheckOutDate);

        // Generate invoice number
        var invoiceNumber = await GenerateInvoiceNumberAsync();

        // Create invoice entity
        var invoice = new Invoice
        {
            InvoiceNumber = invoiceNumber,
            InvoiceDate = DateTime.UtcNow,
            CheckInId = checkInId,
            RoomNumber = billPreview.RoomNumber,
            GuestNames = billPreview.GuestNames,
            CompanyName = billPreview.CompanyName,
            ActualCheckInDate = billPreview.ActualCheckInDate,
            ActualCheckOutDate = actualCheckOutDate,
            TotalNights = billPreview.TotalNights,
            DailyChargesJson = JsonSerializer.Serialize(billPreview.DailyCharges),
            AdditionalChargesJson = billPreview.AdditionalCharges.Any()
                ? JsonSerializer.Serialize(billPreview.AdditionalCharges)
                : null,
            TaxBreakdownJson = JsonSerializer.Serialize(billPreview.TaxBreakdown),
            RoomChargesSubtotal = billPreview.RoomChargesSubtotal,
            MealChargesSubtotal = billPreview.MealChargesSubtotal,
            DiscountAmount = billPreview.DiscountAmount,
            AdditionalChargesSubtotal = billPreview.AdditionalChargesSubtotal,
            SubtotalBeforeTax = billPreview.SubtotalBeforeTax,
            TotalTax = billPreview.TotalTax,
            GrandTotal = billPreview.GrandTotal,
            PaymentStatus = dto.PaymentStatus,
            PaymentMethod = dto.PaymentMethod,
            PaymentDate = dto.PaymentStatus == "Paid" ? (dto.PaymentDate ?? DateTime.UtcNow) : dto.PaymentDate,
            PaymentNotes = dto.PaymentNotes,
            CreatedAt = DateTime.UtcNow
        };

        // Calculate TotalPaid/BalanceDue from unified Payment records
        var totalPaid = await _context.Payments
            .Where(p => p.CheckInId == checkInId && p.DeletedAt == null)
            .SumAsync(p => p.PaymentType == Models.Enums.PaymentType.Refund ? -p.Amount : p.Amount);

        invoice.TotalPaid = totalPaid;
        invoice.BalanceDue = invoice.GrandTotal - totalPaid;

        // Derive PaymentStatus from actual payments if not explicitly set
        if (string.IsNullOrEmpty(dto.PaymentStatus) || dto.PaymentStatus == "Unpaid")
        {
            invoice.PaymentStatus = totalPaid >= invoice.GrandTotal ? "Paid"
                : totalPaid > 0 ? "PartiallyPaid"
                : "Unpaid";
        }

        _context.Invoices.Add(invoice);

        // Update check-in status
        checkIn.Status = CheckInStatus.CheckedOut;
        checkIn.UpdatedAt = DateTime.UtcNow;

        // Check for other active check-ins on the same room (shared room logic)
        var otherActiveCheckIns = await _context.CheckIns
            .Where(c => c.RoomId == checkIn.RoomId && c.Status == CheckInStatus.Active && c.Id != checkInId)
            .ToListAsync();

        if (otherActiveCheckIns.Any())
        {
            // Room stays Occupied, update date range to cover remaining check-ins
            if (checkIn.Room != null)
            {
                checkIn.Room.RoomStatusFromDate = otherActiveCheckIns.Min(c => c.CheckInDate);
                checkIn.Room.RoomStatusToDate = otherActiveCheckIns.Max(c => c.CheckOutDate);
            }

            // If only 1 remains, un-share it
            if (otherActiveCheckIns.Count == 1)
            {
                otherActiveCheckIns[0].IsSharedRoom = false;
                otherActiveCheckIns[0].SharedGroupId = null;
            }
        }
        else
        {
            // No other active check-ins: set room to Dirty
            if (checkIn.Room != null)
            {
                checkIn.Room.RoomStatus = RoomStatus.Dirty;
                checkIn.Room.RoomStatusFromDate = null;
                checkIn.Room.RoomStatusToDate = null;
            }
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Invoice {InvoiceNumber} created for check-in {CheckInId}", invoiceNumber, checkInId);

        return await GetInvoiceByIdAsync(invoice.Id)
            ?? throw new Exception("Failed to retrieve created invoice");
    }

    public async Task<InvoiceDto?> GetInvoiceByIdAsync(int invoiceId)
    {
        var invoice = await _context.Invoices
            .FirstOrDefaultAsync(i => i.Id == invoiceId && i.DeletedAt == null);

        if (invoice == null)
            return null;

        var pax = await _context.Guests
            .CountAsync(g => g.CheckInId == invoice.CheckInId && g.DeletedAt == null);

        return new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            InvoiceDate = invoice.InvoiceDate,
            CheckInId = invoice.CheckInId,
            RoomNumber = invoice.RoomNumber,
            GuestNames = invoice.GuestNames,
            CompanyName = invoice.CompanyName,
            ActualCheckInDate = invoice.ActualCheckInDate,
            ActualCheckOutDate = invoice.ActualCheckOutDate,
            TotalNights = invoice.TotalNights,
            Pax = pax,
            DailyCharges = JsonSerializer.Deserialize<List<DailyChargeDto>>(invoice.DailyChargesJson) ?? new(),
            AdditionalCharges = !string.IsNullOrEmpty(invoice.AdditionalChargesJson)
                ? JsonSerializer.Deserialize<List<AdditionalChargeDto>>(invoice.AdditionalChargesJson) ?? new()
                : new(),
            RoomChargesSubtotal = invoice.RoomChargesSubtotal,
            MealChargesSubtotal = invoice.MealChargesSubtotal,
            DiscountAmount = invoice.DiscountAmount,
            AdditionalChargesSubtotal = invoice.AdditionalChargesSubtotal,
            SubtotalBeforeTax = invoice.SubtotalBeforeTax,
            TaxBreakdown = JsonSerializer.Deserialize<List<TaxSummaryDto>>(invoice.TaxBreakdownJson) ?? new(),
            TotalTax = invoice.TotalTax,
            GrandTotal = invoice.GrandTotal,
            TotalPaid = invoice.TotalPaid,
            BalanceDue = invoice.BalanceDue,
            PaymentStatus = invoice.PaymentStatus,
            PaymentMethod = invoice.PaymentMethod,
            PaymentDate = invoice.PaymentDate,
            PaymentNotes = invoice.PaymentNotes,
            CreatedAt = invoice.CreatedAt,
            CreatedBy = invoice.CreatedBy
        };
    }

    public async Task<InvoiceDto?> GetInvoiceByCheckInIdAsync(int checkInId)
    {
        var invoice = await _context.Invoices
            .FirstOrDefaultAsync(i => i.CheckInId == checkInId && i.DeletedAt == null);

        if (invoice == null)
            return null;

        return await GetInvoiceByIdAsync(invoice.Id);
    }

    public async Task<AdditionalChargeDto> AddChargeAsync(int checkInId, CreateAdditionalChargeDto dto, string? addedBy = null)
    {
        // Validate check-in exists and is active
        var checkIn = await _context.CheckIns
            .FirstOrDefaultAsync(c => c.Id == checkInId && c.DeletedAt == null);

        if (checkIn == null)
            throw new NotFoundException(nameof(CheckIn), checkInId);

        if (checkIn.Status != CheckInStatus.Active)
            throw new BusinessRuleException("Cannot add charges to non-active check-in");

        // Validate charge date is not in future
        if (dto.ChargeDate.Date > DateTime.Today)
            throw new BusinessRuleException("Charge date cannot be in the future");

        // Validate charge date is within check-in period
        var checkInDate = (checkIn.ActualCheckInDate ?? checkIn.CheckInDate).Date;
        if (dto.ChargeDate.Date < checkInDate)
            throw new BusinessRuleException("Charge date cannot be before check-in date");

        var charge = new AdditionalCharge
        {
            CheckInId = checkInId,
            ChargeDate = dto.ChargeDate,
            ChargeType = dto.ChargeType,
            Description = dto.Description,
            Amount = dto.Amount,
            Quantity = dto.Quantity,
            ApplyTax = dto.ApplyTax,
            VoucherTaxConfigId = dto.VoucherTaxConfigId,
            AddedBy = addedBy,
            CreatedAt = DateTime.UtcNow
        };

        _context.AdditionalCharges.Add(charge);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Additional charge added to check-in {CheckInId}: {ChargeType} - {Amount} (ApplyTax: {ApplyTax})",
            checkInId, dto.ChargeType, dto.Amount, dto.ApplyTax);

        return new AdditionalChargeDto
        {
            Id = charge.Id,
            CheckInId = charge.CheckInId,
            ChargeDate = charge.ChargeDate,
            ChargeType = charge.ChargeType,
            Description = charge.Description,
            Amount = charge.Amount,
            Quantity = charge.Quantity,
            TotalAmount = charge.Amount * charge.Quantity,
            ApplyTax = charge.ApplyTax,
            VoucherTaxConfigId = charge.VoucherTaxConfigId,
            AddedBy = charge.AddedBy,
            CreatedAt = charge.CreatedAt
        };
    }

    public async Task<List<AdditionalChargeDto>> GetChargesByCheckInAsync(int checkInId)
    {
        var charges = await _context.AdditionalCharges
            .Include(a => a.VoucherTaxConfig)
            .Where(a => a.CheckInId == checkInId && a.DeletedAt == null)
            .OrderBy(a => a.ChargeDate)
            .ThenBy(a => a.CreatedAt)
            .ToListAsync();

        return charges.Select(c => new AdditionalChargeDto
        {
            Id = c.Id,
            CheckInId = c.CheckInId,
            ChargeDate = c.ChargeDate,
            ChargeType = c.ChargeType,
            Description = c.Description,
            Amount = c.Amount,
            Quantity = c.Quantity,
            TotalAmount = c.TotalAmount,
            ApplyTax = c.ApplyTax,
            VoucherTaxConfigId = c.VoucherTaxConfigId,
            AddedBy = c.AddedBy,
            CreatedAt = c.CreatedAt
        }).ToList();
    }

    public async Task<AdditionalChargeDto?> UpdateChargeAsync(int chargeId, UpdateAdditionalChargeDto dto)
    {
        var charge = await _context.AdditionalCharges
            .Include(a => a.CheckIn)
            .FirstOrDefaultAsync(a => a.Id == chargeId && a.DeletedAt == null);

        if (charge == null)
            return null;

        // Validate check-in is still active
        if (charge.CheckIn.Status != CheckInStatus.Active)
            throw new BusinessRuleException("Cannot update charges for non-active check-in");

        charge.ChargeDate = dto.ChargeDate;
        charge.ChargeType = dto.ChargeType;
        charge.Description = dto.Description;
        charge.Amount = dto.Amount;
        charge.Quantity = dto.Quantity;
        charge.ApplyTax = dto.ApplyTax;
        charge.VoucherTaxConfigId = dto.VoucherTaxConfigId;
        charge.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new AdditionalChargeDto
        {
            Id = charge.Id,
            CheckInId = charge.CheckInId,
            ChargeDate = charge.ChargeDate,
            ChargeType = charge.ChargeType,
            Description = charge.Description,
            Amount = charge.Amount,
            Quantity = charge.Quantity,
            TotalAmount = charge.TotalAmount,
            ApplyTax = charge.ApplyTax,
            VoucherTaxConfigId = charge.VoucherTaxConfigId,
            AddedBy = charge.AddedBy,
            CreatedAt = charge.CreatedAt
        };
    }

    public async Task<bool> DeleteChargeAsync(int chargeId, string? deletedBy = null)
    {
        var charge = await _context.AdditionalCharges
            .Include(a => a.CheckIn)
            .FirstOrDefaultAsync(a => a.Id == chargeId && a.DeletedAt == null);

        if (charge == null)
            return false;

        // Validate check-in is still active
        if (charge.CheckIn.Status != CheckInStatus.Active)
            throw new BusinessRuleException("Cannot delete charges for non-active check-in");

        charge.DeletedAt = DateTime.UtcNow;
        charge.DeletedBy = deletedBy;

        await _context.SaveChangesAsync();

        return true;
    }

    // Private helper methods

    private async Task<List<DailyChargeDto>> CalculateDailyChargesAsync(
        int roomTypeId,
        int occupancyCount,
        DateTime checkInDate,
        DateTime checkOutDate,
        int? companyId,
        int? mealPlanId,
        decimal discountPercentage,
        TaxType taxType,
        TaxSlabSnapshot? taxSnapshot,
        decimal fallbackRoomRate = 0,
        decimal fallbackMealRate = 0)
    {
        var dailyCharges = new List<DailyChargeDto>();
        var currentDate = checkInDate.Date;
        var endDate = checkOutDate.Date;

        decimal? previousRoomRate = null;

        while (currentDate < endDate)
        {
            // Get applicable tariff for this date (with fallback to stored value)
            var roomRate = await GetApplicableTariffForDateAsync(roomTypeId, occupancyCount, currentDate, companyId, fallbackRoomRate);

            // Get meal plan rate for this date (with fallback to stored value)
            var mealPlanRate = mealPlanId.HasValue
                ? await GetApplicableMealPlanRateForDateAsync(mealPlanId.Value, roomTypeId, currentDate, occupancyCount, fallbackMealRate)
                : 0;

            // Calculate daily amounts
            var subtotalBeforeDiscount = roomRate + mealPlanRate;
            var discountAmount = subtotalBeforeDiscount * discountPercentage / 100;
            var subtotalAfterDiscount = subtotalBeforeDiscount - discountAmount;

            // Calculate taxes using slab-based system
            var taxes = await _taxService.CalculateTaxAsync(subtotalAfterDiscount, taxType, currentDate, taxSnapshot);
            var totalTax = taxes.Sum(t => t.TaxAmount);
            var dayTotal = subtotalAfterDiscount + totalTax;

            // Check for rate change
            string? rateChangeNote = null;
            if (previousRoomRate.HasValue && previousRoomRate.Value != roomRate)
            {
                rateChangeNote = "Rate change effective";
            }
            previousRoomRate = roomRate;

            dailyCharges.Add(new DailyChargeDto
            {
                Date = currentDate,
                RoomRate = roomRate,
                MealPlanRate = mealPlanRate,
                SubtotalBeforeDiscount = subtotalBeforeDiscount,
                DiscountPercentage = discountPercentage,
                DiscountAmount = discountAmount,
                SubtotalAfterDiscount = subtotalAfterDiscount,
                Taxes = taxes,
                TotalTax = totalTax,
                DayTotal = dayTotal,
                RateChangeNote = rateChangeNote
            });

            currentDate = currentDate.AddDays(1);
        }

        return dailyCharges;
    }

    private async Task<decimal> GetApplicableTariffForDateAsync(
        int roomTypeId,
        int occupancyCount,
        DateTime date,
        int? companyId,
        decimal fallbackRate = 0)
    {
        // Check for company-specific tariff first
        if (companyId.HasValue)
        {
            var companyTariff = await _context.CompanyTariffs
                .Where(ct => ct.CompanyId == companyId.Value &&
                            ct.RoomTypeId == roomTypeId &&
                            ct.IsActive &&
                            ct.DeletedAt == null &&
                            ct.EffectiveFrom <= date &&
                            (ct.EffectiveTo == null || ct.EffectiveTo >= date))
                .OrderByDescending(ct => ct.EffectiveFrom)
                .FirstOrDefaultAsync();

            if (companyTariff != null)
                return companyTariff.RatePerNight;
        }

        // Fall back to base tariff
        var baseTariff = await _context.BaseTariffs
            .Where(bt => bt.RoomTypeId == roomTypeId &&
                        bt.OccupancyCount == occupancyCount &&
                        bt.IsActive &&
                        bt.DeletedAt == null &&
                        bt.EffectiveFrom <= date &&
                        (bt.EffectiveTo == null || bt.EffectiveTo >= date))
            .OrderByDescending(bt => bt.EffectiveFrom)
            .FirstOrDefaultAsync();

        if (baseTariff == null)
        {
            // If no tariff found and no fallback provided, throw exception
            if (fallbackRate == 0)
            {
                var roomType = await _context.RoomTypes.FindAsync(roomTypeId);
                throw new BusinessRuleException(
                    $"No base tariff found for {roomType?.RoomType ?? "room"} with {occupancyCount} occupant(s) on {date:yyyy-MM-dd}");
            }

            // Use fallback rate (from stored CheckIn.TariffApplied)
            return fallbackRate;
        }

        return baseTariff.RatePerNight;
    }

    private async Task<decimal> GetApplicableMealPlanRateForDateAsync(
        int mealPlanId,
        int roomTypeId,
        DateTime date,
        int occupancyCount,
        decimal fallbackRate = 0)
    {
        var mealPlanRate = await _context.MealPlanRates
            .Where(mpr => mpr.MealPlanId == mealPlanId &&
                         mpr.RoomTypeId == roomTypeId &&
                         mpr.IsActive &&
                         mpr.DeletedAt == null &&
                         mpr.EffectiveFrom <= date &&
                         (mpr.EffectiveTo == null || mpr.EffectiveTo >= date))
            .OrderByDescending(mpr => mpr.EffectiveFrom)
            .FirstOrDefaultAsync();

        if (mealPlanRate == null)
            return fallbackRate; // Use stored CheckIn.MealPlanRate as fallback

        return mealPlanRate.RatePerPersonPerNight * occupancyCount;
    }

    private async Task<List<TaxLineDto>> CalculateTaxForAmountAsync(decimal taxableAmount, DateTime date)
    {
        // Get active tax configurations
        var taxConfigs = await _context.TaxConfigurations
            .Where(tc => tc.IsActive &&
                        tc.DeletedAt == null &&
                        tc.EffectiveFrom <= date &&
                        (tc.EffectiveTo == null || tc.EffectiveTo >= date))
            .ToListAsync();

        // If no tax config, use defaults (CGST 9% + SGST 9%)
        if (!taxConfigs.Any())
        {
            return new List<TaxLineDto>
            {
                new() { TaxType = "CGST", TaxPercentage = 9.0m, TaxableAmount = taxableAmount, TaxAmount = taxableAmount * 0.09m },
                new() { TaxType = "SGST", TaxPercentage = 9.0m, TaxableAmount = taxableAmount, TaxAmount = taxableAmount * 0.09m }
            };
        }

        return taxConfigs.Select(tc => new TaxLineDto
        {
            TaxType = tc.TaxType,
            TaxPercentage = tc.TaxPercentage,
            TaxableAmount = taxableAmount,
            TaxAmount = Math.Round(taxableAmount * tc.TaxPercentage / 100, 2)
        }).ToList();
    }

    private async Task<List<TaxLineDto>> CalculateTaxForAdditionalChargesAsync(
        List<AdditionalCharge> charges,
        TaxType checkInTaxType,
        DateTime date,
        TaxSlabSnapshot? taxSnapshot)
    {
        var allTaxLines = new List<TaxLineDto>();

        foreach (var charge in charges)
        {
            // Skip if tax is not applied
            if (!charge.ApplyTax)
                continue;

            List<TaxLineDto> taxLines;

            if (charge.VoucherTaxConfigId.HasValue && charge.VoucherTaxConfig != null)
            {
                // Use voucher-specific tax rates
                var voucherTaxConfig = charge.VoucherTaxConfig;

                if (checkInTaxType == TaxType.Igst)
                {
                    taxLines = new List<TaxLineDto>
                    {
                        new TaxLineDto
                        {
                            TaxType = "IGST",
                            TaxPercentage = voucherTaxConfig.IgstPercentage,
                            TaxableAmount = charge.TotalAmount,
                            TaxAmount = Math.Round(charge.TotalAmount * voucherTaxConfig.IgstPercentage / 100, 2)
                        }
                    };
                }
                else
                {
                    taxLines = new List<TaxLineDto>
                    {
                        new TaxLineDto
                        {
                            TaxType = "CGST",
                            TaxPercentage = voucherTaxConfig.CgstPercentage,
                            TaxableAmount = charge.TotalAmount,
                            TaxAmount = Math.Round(charge.TotalAmount * voucherTaxConfig.CgstPercentage / 100, 2)
                        },
                        new TaxLineDto
                        {
                            TaxType = "SGST",
                            TaxPercentage = voucherTaxConfig.SgstPercentage,
                            TaxableAmount = charge.TotalAmount,
                            TaxAmount = Math.Round(charge.TotalAmount * voucherTaxConfig.SgstPercentage / 100, 2)
                        }
                    };
                }
            }
            else
            {
                // Use check-in tax type with tax slab system
                taxLines = await _taxService.CalculateTaxAsync(
                    charge.TotalAmount,
                    checkInTaxType,
                    date,
                    taxSnapshot);
            }

            allTaxLines.AddRange(taxLines);
        }

        return allTaxLines;
    }

    private List<TaxSummaryDto> AggregateTaxBreakdown(List<DailyChargeDto> dailyCharges, List<TaxLineDto> additionalTaxes)
    {
        var allTaxes = dailyCharges.SelectMany(d => d.Taxes).Concat(additionalTaxes);

        return allTaxes
            .GroupBy(t => new { t.TaxType, t.TaxPercentage })
            .Select(g => new TaxSummaryDto
            {
                TaxType = g.Key.TaxType,
                TaxPercentage = g.Key.TaxPercentage,
                TotalTaxAmount = g.Sum(t => t.TaxAmount)
            })
            .ToList();
    }

    private async Task<string> GenerateInvoiceNumberAsync()
    {
        var year = DateTime.UtcNow.Year;
        var prefix = $"INV-{year}-";

        // Get the latest invoice for this year
        var lastInvoice = await _context.Invoices
            .Where(i => i.InvoiceNumber.StartsWith(prefix))
            .OrderByDescending(i => i.Id)
            .FirstOrDefaultAsync();

        int nextNumber = 1;
        if (lastInvoice != null)
        {
            var lastNumberStr = lastInvoice.InvoiceNumber.Substring(prefix.Length);
            if (int.TryParse(lastNumberStr, out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        return $"{prefix}{nextNumber:D5}";
    }
}
