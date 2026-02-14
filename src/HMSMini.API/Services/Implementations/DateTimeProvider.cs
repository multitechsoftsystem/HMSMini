using HMSMini.API.Data;
using HMSMini.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HMSMini.API.Services.Implementations;

/// <summary>
/// Implementation of IDateTimeProvider that provides working date from SystemSettings
/// </summary>
public class DateTimeProvider : IDateTimeProvider
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DateTimeProvider> _logger;

    public DateTimeProvider(
        ApplicationDbContext context,
        ILogger<DateTimeProvider> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Current UTC time for audit timestamps
    /// </summary>
    public DateTime UtcNow => DateTime.UtcNow;

    /// <summary>
    /// Current system date
    /// </summary>
    public DateTime Today => DateTime.Today;

    /// <summary>
    /// Gets the current business/working date from SystemSettings
    /// Falls back to system date if setting not found
    /// </summary>
    public async Task<DateTime> GetWorkingDateAsync()
    {
        try
        {
            var setting = await _context.SystemSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SettingKey == "WorkingDate");

            if (setting == null)
            {
                _logger.LogWarning("WorkingDate setting not found in SystemSettings. Falling back to system date.");
                return DateTime.Today;
            }

            if (DateTime.TryParse(setting.SettingValue, out DateTime workingDate))
            {
                return workingDate.Date; // Ensure time component is stripped
            }

            _logger.LogError("Invalid WorkingDate value in SystemSettings: {Value}. Falling back to system date.", setting.SettingValue);
            return DateTime.Today;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving WorkingDate from SystemSettings. Falling back to system date.");
            return DateTime.Today;
        }
    }
}
