namespace HMSMini.API.Services.Interfaces;

/// <summary>
/// Abstraction for DateTime access to support working date concept
/// Replaces direct DateTime.UtcNow and DateTime.Today calls throughout the system
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Current UTC time for audit timestamps
    /// Use this instead of DateTime.UtcNow
    /// </summary>
    DateTime UtcNow { get; }

    /// <summary>
    /// Current system date (not the business/working date)
    /// Use this instead of DateTime.Today for non-business logic
    /// </summary>
    DateTime Today { get; }

    /// <summary>
    /// Gets the current business/working date for hotel operations
    /// This is the date used for check-ins, voucher posting, and all business transactions
    /// </summary>
    /// <returns>Current working date from SystemSettings</returns>
    Task<DateTime> GetWorkingDateAsync();
}
