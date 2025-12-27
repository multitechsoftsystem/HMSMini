namespace HMSMini.API.Models.Enums;

/// <summary>
/// Represents the status of a check-in record
/// </summary>
public enum CheckInStatus
{
    /// <summary>
    /// Guest is currently checked in
    /// </summary>
    Active = 0,

    /// <summary>
    /// Guest has checked out
    /// </summary>
    CheckedOut = 1,

    /// <summary>
    /// Check-in was cancelled
    /// </summary>
    Cancelled = 2
}
