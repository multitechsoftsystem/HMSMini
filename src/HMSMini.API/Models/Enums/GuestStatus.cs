namespace HMSMini.API.Models.Enums;

/// <summary>
/// Status of individual guest
/// </summary>
public enum GuestStatus
{
    /// <summary>
    /// Guest is currently checked in and active
    /// </summary>
    Active = 0,

    /// <summary>
    /// Guest has checked out
    /// </summary>
    CheckedOut = 1
}
