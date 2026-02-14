using HMSMini.API.Models.DTOs.Voucher;

namespace HMSMini.API.Services.Interfaces;

/// <summary>
/// Service for managing vouchers (accounting ledger entries)
/// </summary>
public interface IVoucherService
{
    /// <summary>
    /// Gets a voucher by ID
    /// </summary>
    Task<VoucherDto?> GetByIdAsync(int id);

    /// <summary>
    /// Gets all vouchers for a check-in
    /// </summary>
    Task<List<VoucherDto>> GetByCheckInIdAsync(int checkInId);

    /// <summary>
    /// Gets vouchers by date range
    /// </summary>
    Task<List<VoucherDto>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);

    /// <summary>
    /// Gets voucher summary by type for a date range
    /// </summary>
    Task<List<VoucherSummaryDto>> GetSummaryByDateRangeAsync(DateTime fromDate, DateTime toDate);

    /// <summary>
    /// Creates a manual voucher (Admin/Manager only)
    /// </summary>
    Task<VoucherDto> CreateVoucherAsync(CreateVoucherDto dto);

    /// <summary>
    /// Generates auto-post vouchers for a check-in
    /// CRITICAL: Implements complimentary guest business rule
    /// If GuestType = "Complimentary", NO Room Tariff, Meal Plan, or Tax vouchers are generated
    /// </summary>
    /// <param name="checkInId">Check-in ID to generate vouchers for</param>
    /// <param name="forDate">Date for which to generate vouchers (typically working date)</param>
    /// <param name="postedBy">User posting the vouchers</param>
    /// <returns>List of generated vouchers (empty for complimentary guests)</returns>
    Task<List<VoucherDto>> GenerateAutoPostVouchersForCheckInAsync(int checkInId, DateTime forDate, string? postedBy = null);

    /// <summary>
    /// Posts unposted additional charges as vouchers
    /// </summary>
    Task<List<VoucherDto>> PostAdditionalChargesAsync(int checkInId, DateTime forDate, string? postedBy = null);

    /// <summary>
    /// Cancels a voucher (Admin/Manager only)
    /// Creates a reversing entry
    /// </summary>
    Task<VoucherDto> CancelVoucherAsync(int voucherId, CancelVoucherDto dto);

    /// <summary>
    /// Generates the next voucher number
    /// Format: VCH-YYYYMMDD-NNNN
    /// </summary>
    Task<string> GenerateVoucherNumberAsync(DateTime voucherDate);
}
