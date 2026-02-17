namespace HMSMini.API.Models.Enums;

/// <summary>
/// User roles for authorization
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Administrator - Full access to all features
    /// </summary>
    Admin = 0,

    /// <summary>
    /// Manager - Can view reports and manage operations
    /// </summary>
    Manager = 1,

    /// <summary>
    /// Receptionist - Can handle check-ins, check-outs, and guest management
    /// </summary>
    Receptionist = 2,

    /// <summary>
    /// Housekeeping - Can manage room cleaning and status updates
    /// </summary>
    Housekeeping = 3,

    /// <summary>
    /// Maintenance - Can manage room maintenance and repairs
    /// </summary>
    Maintenance = 4,

    /// <summary>
    /// BanquetManager - Full access to banquet module
    /// </summary>
    BanquetManager = 5,

    /// <summary>
    /// BanquetStaff - Banquet operations (no masters, no edit invoice)
    /// </summary>
    BanquetStaff = 6,

    /// <summary>
    /// Developer - Software vendor support account. Full access to system-locked settings and feature configuration.
    /// </summary>
    Developer = 7
}
