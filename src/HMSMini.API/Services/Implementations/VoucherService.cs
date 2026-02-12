using HMSMini.API.Data;
using HMSMini.API.Models.DTOs.Voucher;
using HMSMini.API.Models.Entities;
using HMSMini.API.Models.Enums;
using HMSMini.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HMSMini.API.Services.Implementations;

/// <summary>
/// Service for managing vouchers with complimentary guest business rules
/// </summary>
public class VoucherService : IVoucherService
{
    private readonly ApplicationDbContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<VoucherService> _logger;

    public VoucherService(
        ApplicationDbContext context,
        IDateTimeProvider dateTimeProvider,
        ILogger<VoucherService> logger)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<VoucherDto?> GetByIdAsync(int id)
    {
        var voucher = await _context.Vouchers
            .Include(v => v.Guest)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id);

        if (voucher == null)
            return null;

        return MapToDto(voucher);
    }

    public async Task<List<VoucherDto>> GetByCheckInIdAsync(int checkInId)
    {
        var vouchers = await _context.Vouchers
            .Include(v => v.Guest)
            .Where(v => v.CheckInId == checkInId)
            .OrderBy(v => v.VoucherDate)
            .ThenBy(v => v.VoucherNumber)
            .AsNoTracking()
            .ToListAsync();

        return vouchers.Select(MapToDto).ToList();
    }

    public async Task<List<VoucherDto>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
    {
        var vouchers = await _context.Vouchers
            .Include(v => v.Guest)
            .Where(v => v.VoucherDate >= fromDate.Date && v.VoucherDate <= toDate.Date)
            .OrderBy(v => v.VoucherDate)
            .ThenBy(v => v.VoucherNumber)
            .AsNoTracking()
            .ToListAsync();

        return vouchers.Select(MapToDto).ToList();
    }

    public async Task<List<VoucherSummaryDto>> GetSummaryByDateRangeAsync(DateTime fromDate, DateTime toDate)
    {
        var summary = await _context.Vouchers
            .Where(v => v.VoucherDate >= fromDate.Date && v.VoucherDate <= toDate.Date)
            .Where(v => v.PostingStatus == VoucherPostingStatus.Posted)
            .GroupBy(v => v.VoucherType)
            .Select(g => new VoucherSummaryDto
            {
                VoucherType = g.Key,
                Count = g.Count(),
                TotalAmount = g.Sum(v => v.Amount)
            })
            .OrderBy(s => s.VoucherType)
            .AsNoTracking()
            .ToListAsync();

        return summary;
    }

    public async Task<VoucherDto> CreateVoucherAsync(CreateVoucherDto dto)
    {
        var workingDate = await _dateTimeProvider.GetWorkingDateAsync();
        var voucherNumber = await GenerateVoucherNumberAsync(workingDate);

        var voucher = new Voucher
        {
            VoucherNumber = voucherNumber,
            VoucherDate = workingDate,
            PostingDate = workingDate,
            VoucherType = dto.VoucherType,
            Description = dto.Description,
            Amount = dto.Amount,
            CheckInId = dto.CheckInId,
            GuestId = dto.GuestId,
            RoomNumber = dto.RoomNumber,
            PostingStatus = VoucherPostingStatus.Posted,
            AutoPostDaily = dto.AutoPostDaily,
            TaxType = dto.TaxType,
            TaxPercentage = dto.TaxPercentage,
            TaxableAmount = dto.TaxableAmount,
            AdditionalChargeId = dto.AdditionalChargeId,
            PostedAt = _dateTimeProvider.UtcNow,
            PostedBy = dto.PostedBy,
            CreatedBy = dto.PostedBy
        };

        _context.Vouchers.Add(voucher);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Manual voucher {VoucherNumber} created for CheckIn {CheckInId} by {User}",
            voucherNumber,
            dto.CheckInId,
            dto.PostedBy ?? "System");

        return MapToDto(voucher);
    }

    /// <summary>
    /// CRITICAL METHOD: Generates auto-post vouchers for a check-in
    /// Implements THE most important business rule: NO charges for complimentary guests
    /// </summary>
    public async Task<List<VoucherDto>> GenerateAutoPostVouchersForCheckInAsync(
        int checkInId,
        DateTime forDate,
        string? postedBy = null)
    {
        var checkIn = await _context.CheckIns
            .Include(c => c.GuestType)
            .Include(c => c.Room)
            .Include(c => c.MealPlan)
            .FirstOrDefaultAsync(c => c.Id == checkInId);

        if (checkIn == null)
        {
            _logger.LogWarning("CheckIn {CheckInId} not found for voucher generation", checkInId);
            return new List<VoucherDto>();
        }

        // ==================================================================================
        // CRITICAL BUSINESS RULE: Check if guest is complimentary
        // If GuestType = "Complimentary", NO Room Tariff, Meal Plan, or Tax vouchers
        // ==================================================================================
        bool isComplimentary = checkIn.GuestType?.TypeName?
            .Equals("Complimentary", StringComparison.OrdinalIgnoreCase) ?? false;

        if (isComplimentary)
        {
            _logger.LogInformation(
                "Skipping auto-post vouchers for complimentary guest CheckIn {CheckInId}. " +
                "Guest Type: {GuestType}. No Room Tariff, Meal Plan, or Tax charges will be posted.",
                checkInId,
                checkIn.GuestType?.TypeName);

            return new List<VoucherDto>();
        }

        // Generate vouchers for non-complimentary guests
        var vouchers = new List<Voucher>();
        var roomNumber = checkIn.Room?.RoomNumber ?? "Unknown";

        // 1. Room Tariff Voucher
        if (checkIn.TariffApplied.HasValue && checkIn.TariffApplied.Value > 0)
        {
            var roomTariff = new Voucher
            {
                VoucherNumber = await GenerateVoucherNumberAsync(forDate),
                VoucherDate = forDate,
                PostingDate = forDate,
                VoucherType = VoucherType.RoomTariff,
                Description = $"Room Tariff for {roomNumber} - {forDate:yyyy-MM-dd}",
                Amount = checkIn.TariffApplied.Value,
                CheckInId = checkInId,
                RoomNumber = roomNumber,
                PostingStatus = VoucherPostingStatus.Posted,
                AutoPostDaily = true,
                PostedAt = _dateTimeProvider.UtcNow,
                PostedBy = postedBy,
                CreatedBy = postedBy
            };
            vouchers.Add(roomTariff);
        }

        // 2. Meal Plan Voucher
        if (checkIn.MealPlanRate.HasValue && checkIn.MealPlanRate.Value > 0)
        {
            var mealPlanVoucher = new Voucher
            {
                VoucherNumber = await GenerateVoucherNumberAsync(forDate),
                VoucherDate = forDate,
                PostingDate = forDate,
                VoucherType = VoucherType.MealPlan,
                Description = $"Meal Plan ({checkIn.MealPlan?.PlanName}) for {roomNumber} - {forDate:yyyy-MM-dd}",
                Amount = checkIn.MealPlanRate.Value,
                CheckInId = checkInId,
                RoomNumber = roomNumber,
                PostingStatus = VoucherPostingStatus.Posted,
                AutoPostDaily = true,
                PostedAt = _dateTimeProvider.UtcNow,
                PostedBy = postedBy,
                CreatedBy = postedBy
            };
            vouchers.Add(mealPlanVoucher);
        }

        // 3. Tax Vouchers (based on TaxType and TaxSlabSnapshot)
        if (!string.IsNullOrEmpty(checkIn.TaxSlabSnapshotJson))
        {
            try
            {
                var taxSnapshot = JsonSerializer.Deserialize<TaxSlabSnapshot>(checkIn.TaxSlabSnapshotJson);

                if (taxSnapshot != null && taxSnapshot.Slabs.Any())
                {
                    // Calculate taxable base (room tariff + meal plan - discount)
                    decimal taxableAmount = (checkIn.TariffApplied ?? 0) + (checkIn.MealPlanRate ?? 0);

                    if (checkIn.DiscountPercentage > 0)
                    {
                        taxableAmount = taxableAmount * (1 - checkIn.DiscountPercentage / 100);
                    }

                    // Find applicable tax slab
                    var applicableSlab = taxSnapshot.Slabs
                        .Where(s => taxableAmount >= s.MinAmount && (!s.MaxAmount.HasValue || taxableAmount <= s.MaxAmount.Value))
                        .FirstOrDefault();

                    if (applicableSlab != null)
                    {
                        if (checkIn.TaxType == Models.Enums.TaxType.Igst)
                        {
                            // Single IGST voucher
                            var igstAmount = Math.Round(taxableAmount * applicableSlab.IgstPercentage / 100, 2);

                            if (igstAmount > 0)
                            {
                                var igstVoucher = new Voucher
                                {
                                    VoucherNumber = await GenerateVoucherNumberAsync(forDate),
                                    VoucherDate = forDate,
                                    PostingDate = forDate,
                                    VoucherType = VoucherType.TaxIGST,
                                    Description = $"IGST @ {applicableSlab.IgstPercentage}% for {roomNumber} - {forDate:yyyy-MM-dd}",
                                    Amount = igstAmount,
                                    CheckInId = checkInId,
                                    RoomNumber = roomNumber,
                                    PostingStatus = VoucherPostingStatus.Posted,
                                    AutoPostDaily = true,
                                    TaxType = "IGST",
                                    TaxPercentage = applicableSlab.IgstPercentage,
                                    TaxableAmount = taxableAmount,
                                    PostedAt = _dateTimeProvider.UtcNow,
                                    PostedBy = postedBy,
                                    CreatedBy = postedBy
                                };
                                vouchers.Add(igstVoucher);
                            }
                        }
                        else
                        {
                            // CGST + SGST
                            var cgstAmount = Math.Round(taxableAmount * applicableSlab.CgstPercentage / 100, 2);
                            var sgstAmount = Math.Round(taxableAmount * applicableSlab.SgstPercentage / 100, 2);

                            if (cgstAmount > 0)
                            {
                                var cgstVoucher = new Voucher
                                {
                                    VoucherNumber = await GenerateVoucherNumberAsync(forDate),
                                    VoucherDate = forDate,
                                    PostingDate = forDate,
                                    VoucherType = VoucherType.TaxCGST,
                                    Description = $"CGST @ {applicableSlab.CgstPercentage}% for {roomNumber} - {forDate:yyyy-MM-dd}",
                                    Amount = cgstAmount,
                                    CheckInId = checkInId,
                                    RoomNumber = roomNumber,
                                    PostingStatus = VoucherPostingStatus.Posted,
                                    AutoPostDaily = true,
                                    TaxType = "CGST",
                                    TaxPercentage = applicableSlab.CgstPercentage,
                                    TaxableAmount = taxableAmount,
                                    PostedAt = _dateTimeProvider.UtcNow,
                                    PostedBy = postedBy,
                                    CreatedBy = postedBy
                                };
                                vouchers.Add(cgstVoucher);
                            }

                            if (sgstAmount > 0)
                            {
                                var sgstVoucher = new Voucher
                                {
                                    VoucherNumber = await GenerateVoucherNumberAsync(forDate),
                                    VoucherDate = forDate,
                                    PostingDate = forDate,
                                    VoucherType = VoucherType.TaxSGST,
                                    Description = $"SGST @ {applicableSlab.SgstPercentage}% for {roomNumber} - {forDate:yyyy-MM-dd}",
                                    Amount = sgstAmount,
                                    CheckInId = checkInId,
                                    RoomNumber = roomNumber,
                                    PostingStatus = VoucherPostingStatus.Posted,
                                    AutoPostDaily = true,
                                    TaxType = "SGST",
                                    TaxPercentage = applicableSlab.SgstPercentage,
                                    TaxableAmount = taxableAmount,
                                    PostedAt = _dateTimeProvider.UtcNow,
                                    PostedBy = postedBy,
                                    CreatedBy = postedBy
                                };
                                vouchers.Add(sgstVoucher);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Failed to deserialize TaxSlabSnapshot for CheckIn {CheckInId}. Skipping tax vouchers.",
                    checkInId);
            }
        }

        if (vouchers.Any())
        {
            _context.Vouchers.AddRange(vouchers);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Generated {Count} auto-post vouchers for CheckIn {CheckInId} on {Date}",
                vouchers.Count,
                checkInId,
                forDate);
        }

        return vouchers.Select(MapToDto).ToList();
    }

    public async Task<List<VoucherDto>> PostAdditionalChargesAsync(int checkInId, DateTime forDate, string? postedBy = null)
    {
        var unpostedCharges = await _context.AdditionalCharges
            .Include(a => a.CheckIn)
            .ThenInclude(c => c.Room)
            .Where(a => a.CheckInId == checkInId)
            .Where(a => !a.IsPostedToVoucher)
            .Where(a => a.DeletedAt == null)
            .ToListAsync();

        if (!unpostedCharges.Any())
            return new List<VoucherDto>();

        var vouchers = new List<Voucher>();

        foreach (var charge in unpostedCharges)
        {
            var voucher = new Voucher
            {
                VoucherNumber = await GenerateVoucherNumberAsync(forDate),
                VoucherDate = forDate,
                PostingDate = forDate,
                VoucherType = VoucherType.AdditionalCharge,
                Description = $"{charge.ChargeType}: {charge.Description}",
                Amount = charge.TotalAmount,
                CheckInId = checkInId,
                RoomNumber = charge.CheckIn.Room?.RoomNumber ?? "Unknown",
                PostingStatus = VoucherPostingStatus.Posted,
                AutoPostDaily = false,
                AdditionalChargeId = charge.Id,
                PostedAt = _dateTimeProvider.UtcNow,
                PostedBy = postedBy,
                CreatedBy = postedBy
            };

            vouchers.Add(voucher);

            // Mark additional charge as posted
            charge.IsPostedToVoucher = true;
            charge.PostedVoucherId = voucher.Id; // Will be set after SaveChanges
        }

        _context.Vouchers.AddRange(vouchers);
        await _context.SaveChangesAsync();

        // Update PostedVoucherId after vouchers have IDs
        for (int i = 0; i < vouchers.Count; i++)
        {
            unpostedCharges[i].PostedVoucherId = vouchers[i].Id;
        }
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Posted {Count} additional charges as vouchers for CheckIn {CheckInId}",
            vouchers.Count,
            checkInId);

        return vouchers.Select(MapToDto).ToList();
    }

    public async Task<VoucherDto> CancelVoucherAsync(int voucherId, CancelVoucherDto dto)
    {
        var voucher = await _context.Vouchers.FindAsync(voucherId);

        if (voucher == null)
            throw new KeyNotFoundException($"Voucher {voucherId} not found.");

        if (voucher.PostingStatus == VoucherPostingStatus.Cancelled)
            throw new InvalidOperationException($"Voucher {voucher.VoucherNumber} is already cancelled.");

        // Update original voucher
        voucher.PostingStatus = VoucherPostingStatus.Cancelled;
        voucher.CancelledAt = _dateTimeProvider.UtcNow;
        voucher.CancelledBy = dto.CancelledBy;
        voucher.CancellationReason = dto.CancellationReason;

        // Create reversing voucher
        var reversingVoucher = new Voucher
        {
            VoucherNumber = await GenerateVoucherNumberAsync(voucher.VoucherDate),
            VoucherDate = voucher.VoucherDate,
            PostingDate = await _dateTimeProvider.GetWorkingDateAsync(),
            VoucherType = VoucherType.Reversal,
            Description = $"Reversal of {voucher.VoucherNumber}: {dto.CancellationReason}",
            Amount = -voucher.Amount, // Negative amount
            CheckInId = voucher.CheckInId,
            GuestId = voucher.GuestId,
            RoomNumber = voucher.RoomNumber,
            PostingStatus = VoucherPostingStatus.Posted,
            AutoPostDaily = false,
            PostedAt = _dateTimeProvider.UtcNow,
            PostedBy = dto.CancelledBy,
            CreatedBy = dto.CancelledBy
        };

        _context.Vouchers.Add(reversingVoucher);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Voucher {VoucherNumber} cancelled by {User}. Reversing voucher {ReversingNumber} created.",
            voucher.VoucherNumber,
            dto.CancelledBy ?? "System",
            reversingVoucher.VoucherNumber);

        return MapToDto(voucher);
    }

    public async Task<string> GenerateVoucherNumberAsync(DateTime voucherDate)
    {
        var datePrefix = voucherDate.ToString("yyyyMMdd");
        var pattern = $"VCH-{datePrefix}-%";

        var lastVoucher = await _context.Vouchers
            .Where(v => EF.Functions.Like(v.VoucherNumber, pattern))
            .OrderByDescending(v => v.VoucherNumber)
            .Select(v => v.VoucherNumber)
            .FirstOrDefaultAsync();

        int sequenceNumber = 1;

        if (lastVoucher != null)
        {
            var parts = lastVoucher.Split('-');
            if (parts.Length == 3 && int.TryParse(parts[2], out int lastSequence))
            {
                sequenceNumber = lastSequence + 1;
            }
        }

        return $"VCH-{datePrefix}-{sequenceNumber:D4}";
    }

    private static VoucherDto MapToDto(Voucher voucher)
    {
        return new VoucherDto
        {
            Id = voucher.Id,
            VoucherNumber = voucher.VoucherNumber,
            VoucherDate = voucher.VoucherDate,
            PostingDate = voucher.PostingDate,
            VoucherType = voucher.VoucherType,
            Description = voucher.Description,
            Amount = voucher.Amount,
            CheckInId = voucher.CheckInId,
            BanquetBookingId = voucher.BanquetBookingId,
            PaymentId = voucher.PaymentId,
            GuestId = voucher.GuestId,
            GuestName = voucher.Guest?.GuestName,
            RoomNumber = voucher.RoomNumber,
            PostingStatus = voucher.PostingStatus,
            AutoPostDaily = voucher.AutoPostDaily,
            TaxType = voucher.TaxType,
            TaxPercentage = voucher.TaxPercentage,
            TaxableAmount = voucher.TaxableAmount,
            AdditionalChargeId = voucher.AdditionalChargeId,
            PostedAt = voucher.PostedAt,
            PostedBy = voucher.PostedBy,
            CancelledAt = voucher.CancelledAt,
            CancelledBy = voucher.CancelledBy,
            CancellationReason = voucher.CancellationReason
        };
    }
}
