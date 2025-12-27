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
    Receptionist = 2
}
