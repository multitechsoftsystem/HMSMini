namespace HMSMini.API.Models.Enums;

/// <summary>
/// Represents the current status of a room
/// </summary>
public enum RoomStatus
{
    /// <summary>
    /// Room is available for check-in
    /// </summary>
    Available = 0,

    /// <summary>
    /// Room is currently occupied by a guest
    /// </summary>
    Occupied = 1,

    /// <summary>
    /// Room is under maintenance
    /// </summary>
    Maintenance = 2,

    /// <summary>
    /// Room is blocked and unavailable
    /// </summary>
    Blocked = 3,

    /// <summary>
    /// Room needs cleaning after checkout
    /// </summary>
    Dirty = 4,

    /// <summary>
    /// Room is reserved for management use
    /// </summary>
    Management = 5
}
