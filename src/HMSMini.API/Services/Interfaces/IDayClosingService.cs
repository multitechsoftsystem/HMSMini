using HMSMini.API.Models.DTOs.DayClosing;

namespace HMSMini.API.Services.Interfaces;

/// <summary>
/// Service for daily closing operations
/// </summary>
public interface IDayClosingService
{
    /// <summary>
    /// Gets the current working date
    /// </summary>
    Task<WorkingDateDto> GetWorkingDateInfoAsync();

    /// <summary>
    /// Validates if the current day can be closed
    /// </summary>
    Task<DayCloseValidationDto> ValidateDayCloseAsync();

    /// <summary>
    /// Gets a preview of vouchers that will be posted during day close
    /// </summary>
    Task<DayClosePreviewDto> GetDayClosePreviewAsync();

    /// <summary>
    /// Executes the day close operation
    /// - Generates auto-post vouchers for all active check-ins (respecting complimentary guest rule)
    /// - Posts unposted additional charges
    /// - Creates audit record
    /// - Increments working date by 1 day
    /// All operations in a single transaction
    /// </summary>
    /// <param name="closedBy">User performing the day close</param>
    /// <returns>Result of day close operation</returns>
    Task<DayCloseResultDto> CloseDayAsync(string? closedBy = null);

    /// <summary>
    /// Gets day closing history
    /// </summary>
    Task<List<DayClosingAuditDto>> GetClosingHistoryAsync(int pageSize = 30, int skip = 0);
}
