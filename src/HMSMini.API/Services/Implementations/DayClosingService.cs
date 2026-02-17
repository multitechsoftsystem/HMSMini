using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.DayClosing;
using HMSMini.API.Models.DTOs.Voucher;
using HMSMini.API.Models.Entities;
using HMSMini.API.Models.Enums;
using HMSMini.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;

namespace HMSMini.API.Services.Implementations;

/// <summary>
/// Service for daily closing operations with transaction safety
/// </summary>
public class DayClosingService : IDayClosingService
{
    private readonly ApplicationDbContext _context;
    private readonly ISystemSettingsService _systemSettingsService;
    private readonly IVoucherService _voucherService;
    private readonly ITaxService _taxService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IJournalEntryService _journalEntryService;
    private readonly IChartOfAccountService _chartOfAccountService;
    private readonly ILogger<DayClosingService> _logger;

    public DayClosingService(
        ApplicationDbContext context,
        ISystemSettingsService systemSettingsService,
        IVoucherService voucherService,
        ITaxService taxService,
        IDateTimeProvider dateTimeProvider,
        IJournalEntryService journalEntryService,
        IChartOfAccountService chartOfAccountService,
        ILogger<DayClosingService> logger)
    {
        _context = context;
        _systemSettingsService = systemSettingsService;
        _voucherService = voucherService;
        _taxService = taxService;
        _dateTimeProvider = dateTimeProvider;
        _journalEntryService = journalEntryService;
        _chartOfAccountService = chartOfAccountService;
        _logger = logger;
    }

    public async Task<WorkingDateDto> GetWorkingDateInfoAsync()
    {
        var workingDate = await _systemSettingsService.GetWorkingDateAsync();
        var systemDate = _dateTimeProvider.Today;

        return new WorkingDateDto
        {
            WorkingDate = workingDate,
            SystemDate = systemDate,
            IsDateClosed = workingDate < systemDate,
            DaysBehindSystemDate = (systemDate - workingDate).Days
        };
    }

    public async Task<DayCloseValidationDto> ValidateDayCloseAsync()
    {
        var workingDate = await _systemSettingsService.GetWorkingDateAsync();
        var validation = new DayCloseValidationDto
        {
            CurrentWorkingDate = workingDate,
            CanClose = true
        };

        // Get system date for comparison
        var systemDate = _dateTimeProvider.Today;

        // CRITICAL VALIDATION: Working date must not equal or exceed system date
        if (workingDate >= systemDate)
        {
            validation.CanClose = false;
            validation.ValidationErrors.Add(
                $"Cannot close day: Working date ({workingDate:yyyy-MM-dd}) must be less than " +
                $"system date ({systemDate:yyyy-MM-dd}). Day closing is only allowed for past dates.");
        }

        // Count active check-ins (only those that were actually in-house on working date)
        var activeCheckIns = await _context.CheckIns
            .Where(c => c.Status == 0) // Active status
            .Where(c => c.CheckInDate.Date <= workingDate
                && (c.ActualCheckOutDate == null || c.ActualCheckOutDate.Value.Date > workingDate))
            .CountAsync();

        validation.ActiveCheckInsCount = activeCheckIns;

        // Count check-ins that will have checkout dates auto-extended (overstays)
        // Only count guests who haven't actually checked out yet
        var checkInsToExtend = await _context.CheckIns
            .Where(c => c.Status == 0)
            .Where(c => c.CheckInDate.Date <= workingDate
                && c.CheckOutDate.Date < workingDate
                && c.ActualCheckOutDate == null)
            .CountAsync();

        validation.CheckInsToExtendCount = checkInsToExtend;

        if (checkInsToExtend > 0)
        {
            validation.ValidationErrors.Add(
                $"Note: {checkInsToExtend} check-in(s) with checkout dates before working date " +
                $"will be auto-extended to {workingDate:yyyy-MM-dd}.");
        }

        // Count unposted additional charges
        var unpostedCharges = await _context.AdditionalCharges
            .Where(a => !a.IsPostedToVoucher)
            .Where(a => a.DeletedAt == null)
            .CountAsync();

        validation.UnpostedAdditionalChargesCount = unpostedCharges;

        // Validation rules
        if (activeCheckIns == 0 && unpostedCharges == 0)
        {
            validation.ValidationErrors.Add("No active check-ins and no unposted charges. Day close may not be necessary.");
        }

        return validation;
    }

    public async Task<DayClosePreviewDto> GetDayClosePreviewAsync()
    {
        var workingDate = await _systemSettingsService.GetWorkingDateAsync();
        var nextWorkingDate = workingDate.AddDays(1);

        var preview = new DayClosePreviewDto
        {
            CurrentWorkingDate = workingDate,
            NextWorkingDate = nextWorkingDate
        };

        // Get all active check-ins (only those that were actually in-house on working date)
        var activeCheckIns = await _context.CheckIns
            .Include(c => c.GuestType)
            .Include(c => c.Room)
            .Include(c => c.Guests)
            .Where(c => c.Status == 0)
            .Where(c => c.CheckInDate.Date <= workingDate
                && (c.ActualCheckOutDate == null || c.ActualCheckOutDate.Value.Date > workingDate))
            .AsNoTracking()
            .ToListAsync();

        // Get all unposted additional charges for preview
        var unpostedChargesByCheckIn = await _context.AdditionalCharges
            .Include(a => a.VoucherTaxConfig)
            .Where(a => !a.IsPostedToVoucher)
            .Where(a => a.DeletedAt == null)
            .Where(a => activeCheckIns.Select(c => c.Id).Contains(a.CheckInId))
            .ToListAsync();
        var chargesGrouped = unpostedChargesByCheckIn.GroupBy(a => a.CheckInId).ToDictionary(g => g.Key, g => g.ToList());

        preview.ActiveCheckInsCount = activeCheckIns.Count;

        // Calculate preview for each check-in
        var voucherTypeSummary = new Dictionary<string, (int count, decimal amount)>();

        foreach (var checkIn in activeCheckIns)
        {
            bool isComplimentary = checkIn.GuestType?.TypeName?
                .Equals("Complimentary", StringComparison.OrdinalIgnoreCase) ?? false;

            var checkInSummary = new CheckInCloseSummary
            {
                CheckInId = checkIn.Id,
                RoomNumber = checkIn.Room?.RoomNumber ?? "Unknown",
                GuestNames = string.Join(", ", checkIn.Guests.Select(g => g.GuestName)),
                GuestType = checkIn.GuestType?.TypeName,
                IsComplimentary = isComplimentary,
                VouchersToPost = 0,
                AmountToPost = 0
            };

            if (!isComplimentary)
            {
                // Room tariff
                if (checkIn.TariffApplied.HasValue && checkIn.TariffApplied.Value > 0)
                {
                    checkInSummary.VouchersToPost++;
                    checkInSummary.AmountToPost += checkIn.TariffApplied.Value;
                    AddToSummary(voucherTypeSummary, "RoomTariff", checkIn.TariffApplied.Value);
                }

                // Meal plan
                if (checkIn.MealPlanRate.HasValue && checkIn.MealPlanRate.Value > 0)
                {
                    checkInSummary.VouchersToPost++;
                    checkInSummary.AmountToPost += checkIn.MealPlanRate.Value;
                    AddToSummary(voucherTypeSummary, "MealPlan", checkIn.MealPlanRate.Value);
                }

                // Tax calculation using TaxService (with automatic fallback to live slabs)
                decimal taxableAmount = (checkIn.TariffApplied ?? 0) + (checkIn.MealPlanRate ?? 0);
                if (checkIn.DiscountPercentage > 0)
                {
                    taxableAmount = taxableAmount * (1 - checkIn.DiscountPercentage / 100);
                }

                if (taxableAmount > 0)
                {
                    try
                    {
                        // Parse tax snapshot if available
                        TaxSlabSnapshot? taxSnapshot = null;
                        if (!string.IsNullOrEmpty(checkIn.TaxSlabSnapshotJson))
                        {
                            try
                            {
                                taxSnapshot = JsonSerializer.Deserialize<TaxSlabSnapshot>(checkIn.TaxSlabSnapshotJson);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex,
                                    "Failed to deserialize TaxSlabSnapshot for CheckIn {CheckInId}, will use live tax slabs",
                                    checkIn.Id);
                            }
                        }

                        // Calculate taxes using TaxService (automatically falls back to live slabs if snapshot is null)
                        var taxLines = await _taxService.CalculateTaxAsync(
                            taxableAmount,
                            checkIn.TaxType,
                            workingDate,
                            taxSnapshot);

                        // Add tax vouchers to summary
                        foreach (var taxLine in taxLines)
                        {
                            checkInSummary.VouchersToPost++;
                            AddToSummary(voucherTypeSummary, $"Tax-{taxLine.TaxType}", taxLine.TaxAmount);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "Failed to calculate tax for CheckIn {CheckInId} during day close preview",
                            checkIn.Id);
                    }
                }

                // Additional charges and their taxes
                if (chargesGrouped.TryGetValue(checkIn.Id, out var additionalCharges))
                {
                    // Parse tax snapshot once for additional charges
                    TaxSlabSnapshot? taxSnapshot = null;
                    if (!string.IsNullOrEmpty(checkIn.TaxSlabSnapshotJson))
                    {
                        try
                        {
                            taxSnapshot = JsonSerializer.Deserialize<TaxSlabSnapshot>(checkIn.TaxSlabSnapshotJson);
                        }
                        catch
                        {
                            // Will fall back to live slabs
                        }
                    }

                    foreach (var charge in additionalCharges.Where(a => !a.IsPostedToVoucher))
                    {
                        // Add voucher for the charge itself
                        checkInSummary.VouchersToPost++;
                        checkInSummary.AmountToPost += charge.TotalAmount;
                        AddToSummary(voucherTypeSummary, "AdditionalCharge", charge.TotalAmount);

                        // Calculate taxes for additional charge
                        if (charge.ApplyTax && charge.TotalAmount > 0)
                        {
                            try
                            {
                                List<(string taxType, decimal amount)> chargeTaxes;

                                if (charge.VoucherTaxConfigId.HasValue && charge.VoucherTaxConfig != null)
                                {
                                    // Use voucher-specific tax rates (e.g., 18% for laundry)
                                    var voucherTaxConfig = charge.VoucherTaxConfig;

                                    if (checkIn.TaxType == Models.Enums.TaxType.Igst)
                                    {
                                        var igstAmount = Math.Round(charge.TotalAmount * voucherTaxConfig.IgstPercentage / 100, 2);
                                        chargeTaxes = new List<(string, decimal)> { ("IGST", igstAmount) };
                                    }
                                    else
                                    {
                                        var cgstAmount = Math.Round(charge.TotalAmount * voucherTaxConfig.CgstPercentage / 100, 2);
                                        var sgstAmount = Math.Round(charge.TotalAmount * voucherTaxConfig.SgstPercentage / 100, 2);
                                        chargeTaxes = new List<(string, decimal)>
                                        {
                                            ("CGST", cgstAmount),
                                            ("SGST", sgstAmount)
                                        };
                                    }
                                }
                                else
                                {
                                    // Use check-in tax slab (fall back to live slabs if snapshot is null)
                                    var taxLines = await _taxService.CalculateTaxAsync(
                                        charge.TotalAmount,
                                        checkIn.TaxType,
                                        workingDate,
                                        taxSnapshot);

                                    chargeTaxes = taxLines.Select(t => (t.TaxType, t.TaxAmount)).ToList();
                                }

                                // Add tax vouchers
                                foreach (var (taxType, taxAmount) in chargeTaxes)
                                {
                                    if (taxAmount > 0)
                                    {
                                        checkInSummary.VouchersToPost++;
                                        AddToSummary(voucherTypeSummary, $"Tax-{taxType}", taxAmount);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex,
                                    "Failed to calculate tax for AdditionalCharge {ChargeId} during day close preview",
                                    charge.Id);
                            }
                        }
                    }
                }
            }

            preview.CheckInSummaries.Add(checkInSummary);
            preview.TotalVouchersToPost += checkInSummary.VouchersToPost;
            preview.TotalRevenueToPost += checkInSummary.AmountToPost;
        }

        // Convert dictionary to summary list
        preview.VoucherSummary = voucherTypeSummary
            .Select(kvp => new VoucherSummaryDto
            {
                VoucherType = kvp.Key,
                Count = kvp.Value.count,
                TotalAmount = kvp.Value.amount
            })
            .OrderBy(s => s.VoucherType)
            .ToList();

        return preview;
    }

    public async Task<DayCloseResultDto> CloseDayAsync(string? closedBy = null)
    {
        var stopwatch = Stopwatch.StartNew();
        var workingDate = await _systemSettingsService.GetWorkingDateAsync();
        var nextWorkingDate = workingDate.AddDays(1);

        // CRITICAL VALIDATION: Block day close if working date >= system date
        var systemDate = _dateTimeProvider.Today;
        if (workingDate >= systemDate)
        {
            var errorMessage = $"Day close blocked: Working date ({workingDate:yyyy-MM-dd}) must be less than " +
                $"system date ({systemDate:yyyy-MM-dd}). Cannot close current or future dates.";

            _logger.LogWarning(
                "Day close blocked for {WorkingDate} by {User}. " +
                "Reason: Working date >= system date ({SystemDate})",
                workingDate,
                closedBy ?? "System",
                systemDate);

            throw new BusinessRuleException(errorMessage);
        }

        _logger.LogInformation(
            "Day close validation passed: Working date {WorkingDate} < System date {SystemDate}",
            workingDate,
            systemDate);

        _logger.LogInformation(
            "Starting day close for {WorkingDate} by {User}",
            workingDate,
            closedBy ?? "System");

        // Begin transaction for atomicity
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var result = new DayCloseResultDto
            {
                ClosedDate = workingDate,
                NewWorkingDate = nextWorkingDate
            };

            // Get all active check-ins
            // Logic: Post tariff for check-ins that were active on the working date
            // - Guest must have checked in on or before working date
            // - Guest must not have checked out before working date (use ActualCheckOutDate if available)
            var activeCheckIns = await _context.CheckIns
                .Include(c => c.GuestType)
                .Include(c => c.Room)
                .Where(c => c.Status == 0)
                .Where(c => c.CheckInDate.Date <= workingDate
                    && (c.ActualCheckOutDate == null || c.ActualCheckOutDate.Value.Date > workingDate))
                .ToListAsync();

            result.TotalActiveCheckIns = activeCheckIns.Count;

            // AUTO-EXTEND CHECKOUT DATES
            // For active check-ins with CheckOutDate < workingDate (overstays), extend to workingDate
            // Only extend if guest hasn't actually checked out yet
            var extendedCheckIns = new List<(int CheckInId, string RoomNumber, DateTime OldDate, DateTime NewDate)>();

            foreach (var checkIn in activeCheckIns)
            {
                if (checkIn.CheckOutDate.Date < workingDate.Date && checkIn.ActualCheckOutDate == null)
                {
                    var oldCheckOutDate = checkIn.CheckOutDate;
                    checkIn.CheckOutDate = workingDate;
                    checkIn.UpdatedAt = _dateTimeProvider.UtcNow;
                    checkIn.UpdatedBy = closedBy;

                    extendedCheckIns.Add((
                        checkIn.Id,
                        checkIn.Room?.RoomNumber ?? "Unknown",
                        oldCheckOutDate,
                        workingDate
                    ));

                    _logger.LogInformation(
                        "Auto-extended checkout: CheckIn {CheckInId}, Room {RoomNumber}, " +
                        "OldDate: {OldDate:yyyy-MM-dd}, NewDate: {NewDate:yyyy-MM-dd}",
                        checkIn.Id,
                        checkIn.Room?.RoomNumber ?? "Unknown",
                        oldCheckOutDate,
                        workingDate);
                }
            }

            // Save checkout date extensions
            if (extendedCheckIns.Any())
            {
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Auto-extended {Count} check-in(s) with checkout dates before working date",
                    extendedCheckIns.Count);
            }

            result.ExtendedCheckInsCount = extendedCheckIns.Count;

            var voucherTypeSummary = new Dictionary<string, (int count, decimal amount)>();
            var checkInSummaries = new List<CheckInCloseSummary>();

            // Generate vouchers for each active check-in
            foreach (var checkIn in activeCheckIns)
            {
                // Generate auto-post vouchers (handles complimentary guest logic)
                var vouchers = await _voucherService.GenerateAutoPostVouchersForCheckInAsync(
                    checkIn.Id,
                    workingDate,
                    closedBy);

                foreach (var voucher in vouchers)
                {
                    AddToSummary(voucherTypeSummary, voucher.VoucherType, voucher.Amount);
                }

                // Post unposted additional charges
                var additionalChargeVouchers = await _voucherService.PostAdditionalChargesAsync(
                    checkIn.Id,
                    workingDate,
                    closedBy);

                foreach (var voucher in additionalChargeVouchers)
                {
                    AddToSummary(voucherTypeSummary, voucher.VoucherType, voucher.Amount);
                }

                result.TotalVouchersPosted += vouchers.Count + additionalChargeVouchers.Count;
                result.TotalRevenuePosted += vouchers.Sum(v => v.Amount) + additionalChargeVouchers.Sum(v => v.Amount);

                // Track check-in summary
                bool isComplimentary = checkIn.GuestType?.TypeName?
                    .Equals("Complimentary", StringComparison.OrdinalIgnoreCase) ?? false;

                checkInSummaries.Add(new CheckInCloseSummary
                {
                    CheckInId = checkIn.Id,
                    RoomNumber = checkIn.Room?.RoomNumber ?? "Unknown",
                    GuestType = checkIn.GuestType?.TypeName,
                    IsComplimentary = isComplimentary,
                    VouchersToPost = vouchers.Count + additionalChargeVouchers.Count,
                    AmountToPost = vouchers.Sum(v => v.Amount) + additionalChargeVouchers.Sum(v => v.Amount)
                });
            }

            // Convert summary to DTO list
            result.VoucherSummary = voucherTypeSummary
                .Select(kvp => new VoucherSummaryDto
                {
                    VoucherType = kvp.Key,
                    Count = kvp.Value.count,
                    TotalAmount = kvp.Value.amount
                })
                .OrderBy(s => s.VoucherType)
                .ToList();

            // Post aggregated journal entry for GL
            try
            {
                await PostDayCloseJournalEntryAsync(workingDate, voucherTypeSummary, closedBy);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to post journal entry for day close {WorkingDate}. Vouchers were still posted successfully.", workingDate);
            }

            // Create audit record
            var audit = new DayClosingAudit
            {
                ClosedDate = workingDate,
                NextWorkingDate = nextWorkingDate,
                TotalActiveCheckIns = result.TotalActiveCheckIns,
                TotalVouchersPosted = result.TotalVouchersPosted,
                TotalRevenuePosted = result.TotalRevenuePosted,
                VoucherSummaryJson = JsonSerializer.Serialize(result.VoucherSummary),
                CheckInSummaryJson = JsonSerializer.Serialize(checkInSummaries),
                ClosingStatus = "Completed",
                ClosedAt = _dateTimeProvider.UtcNow,
                ClosedBy = closedBy,
                DurationSeconds = (int)stopwatch.Elapsed.TotalSeconds
            };

            _context.DayClosingAudits.Add(audit);
            await _context.SaveChangesAsync();

            // Increment working date
            await _systemSettingsService.UpdateWorkingDateAsync(nextWorkingDate, closedBy);

            // Commit transaction
            await transaction.CommitAsync();

            stopwatch.Stop();
            result.Success = true;
            result.DurationSeconds = (int)stopwatch.Elapsed.TotalSeconds;

            _logger.LogInformation(
                "Day close completed successfully. " +
                "Closed: {ClosedDate}, New Working Date: {NewDate}, " +
                "CheckIns: {CheckIns}, Extended: {Extended}, Vouchers: {Vouchers}, " +
                "Revenue: {Revenue:C}, Duration: {Duration}s",
                workingDate,
                nextWorkingDate,
                result.TotalActiveCheckIns,
                result.ExtendedCheckInsCount,
                result.TotalVouchersPosted,
                result.TotalRevenuePosted,
                result.DurationSeconds);

            return result;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            stopwatch.Stop();

            _logger.LogError(ex,
                "Day close failed for {WorkingDate}. Transaction rolled back. Duration: {Duration}s",
                workingDate,
                stopwatch.Elapsed.TotalSeconds);

            // Create failed audit record
            var failedAudit = new DayClosingAudit
            {
                ClosedDate = workingDate,
                NextWorkingDate = nextWorkingDate,
                TotalActiveCheckIns = 0,
                TotalVouchersPosted = 0,
                TotalRevenuePosted = 0,
                ClosingStatus = "Failed",
                ErrorLog = ex.ToString(),
                ClosedAt = _dateTimeProvider.UtcNow,
                ClosedBy = closedBy,
                DurationSeconds = (int)stopwatch.Elapsed.TotalSeconds
            };

            try
            {
                _context.DayClosingAudits.Add(failedAudit);
                await _context.SaveChangesAsync();
            }
            catch (Exception auditEx)
            {
                _logger.LogError(auditEx, "Failed to save failed audit record");
            }

            return new DayCloseResultDto
            {
                Success = false,
                ClosedDate = workingDate,
                NewWorkingDate = nextWorkingDate,
                ErrorMessage = ex.Message,
                DurationSeconds = (int)stopwatch.Elapsed.TotalSeconds
            };
        }
    }

    public async Task<List<DayClosingAuditDto>> GetClosingHistoryAsync(int pageSize = 30, int skip = 0)
    {
        var audits = await _context.DayClosingAudits
            .OrderByDescending(a => a.ClosedDate)
            .Skip(skip)
            .Take(pageSize)
            .Select(a => new DayClosingAuditDto
            {
                Id = a.Id,
                ClosedDate = a.ClosedDate,
                NextWorkingDate = a.NextWorkingDate,
                TotalActiveCheckIns = a.TotalActiveCheckIns,
                TotalVouchersPosted = a.TotalVouchersPosted,
                TotalRevenuePosted = a.TotalRevenuePosted,
                ClosingStatus = a.ClosingStatus,
                ErrorLog = a.ErrorLog,
                ClosedAt = a.ClosedAt,
                ClosedBy = a.ClosedBy,
                DurationSeconds = a.DurationSeconds
            })
            .AsNoTracking()
            .ToListAsync();

        return audits;
    }

    private async Task PostDayCloseJournalEntryAsync(
        DateTime workingDate,
        Dictionary<string, (int count, decimal amount)> voucherTypeSummary,
        string? closedBy)
    {
        if (voucherTypeSummary.Count == 0) return;

        // Cache account IDs
        var arAccountId = await _chartOfAccountService.GetAccountIdByCodeAsync("1003");
        var roomRevenueId = await _chartOfAccountService.GetAccountIdByCodeAsync("4001");
        var mealPlanRevenueId = await _chartOfAccountService.GetAccountIdByCodeAsync("4002");
        var cgstPayableId = await _chartOfAccountService.GetAccountIdByCodeAsync("2002");
        var sgstPayableId = await _chartOfAccountService.GetAccountIdByCodeAsync("2003");
        var igstPayableId = await _chartOfAccountService.GetAccountIdByCodeAsync("2004");
        var additionalRevenueId = await _chartOfAccountService.GetAccountIdByCodeAsync("4006");
        var discountId = await _chartOfAccountService.GetAccountIdByCodeAsync("4007");

        var voucherTypeToAccount = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["RoomTariff"] = roomRevenueId,
            ["MealPlan"] = mealPlanRevenueId,
            ["Tax-CGST"] = cgstPayableId,
            ["Tax-SGST"] = sgstPayableId,
            ["Tax-IGST"] = igstPayableId,
            ["AdditionalCharge"] = additionalRevenueId,
            ["Discount"] = discountId
        };

        var lines = new List<(int accountId, decimal debit, decimal credit, string? desc)>();

        foreach (var (voucherType, (count, amount)) in voucherTypeSummary)
        {
            if (amount == 0) continue;

            if (voucherType == "Discount")
            {
                // Discount: Dr. Discount Given / Cr. AR
                lines.Add((discountId, amount, 0, $"Day close discount ({count} entries)"));
                lines.Add((arAccountId, 0, amount, $"Discount reduction to AR"));
            }
            else if (voucherTypeToAccount.TryGetValue(voucherType, out var creditAccountId))
            {
                // Revenue/Tax: Dr. AR / Cr. Revenue or Tax Payable
                lines.Add((arAccountId, amount, 0, $"Day close {voucherType} ({count} entries)"));
                lines.Add((creditAccountId, 0, amount, $"{voucherType} - day close"));
            }
        }

        if (lines.Count > 0)
        {
            await _journalEntryService.PostJournalEntryAsync(
                workingDate,
                $"Day Close {workingDate:yyyy-MM-dd}",
                JournalSourceType.DayClosing,
                null,
                lines,
                closedBy);
        }
    }

    private static void AddToSummary(
        Dictionary<string, (int count, decimal amount)> summary,
        string voucherType,
        decimal amount)
    {
        if (summary.ContainsKey(voucherType))
        {
            var existing = summary[voucherType];
            summary[voucherType] = (existing.count + 1, existing.amount + amount);
        }
        else
        {
            summary[voucherType] = (1, amount);
        }
    }
}
