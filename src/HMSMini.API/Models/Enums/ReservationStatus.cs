namespace HMSMini.API.Models.Enums;

/// <summary>
/// Status of a reservation
/// </summary>
public enum ReservationStatus
{
    /// <summary>
    /// Reservation is pending confirmation
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Reservation has been confirmed
    /// </summary>
    Confirmed = 1,

    /// <summary>
    /// Guest has checked in (reservation fulfilled)
    /// </summary>
    CheckedIn = 2,

    /// <summary>
    /// Reservation was cancelled
    /// </summary>
    Cancelled = 3,

    /// <summary>
    /// Guest did not show up
    /// </summary>
    NoShow = 4
}
