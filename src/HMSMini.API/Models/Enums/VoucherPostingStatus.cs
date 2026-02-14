namespace HMSMini.API.Models.Enums;

/// <summary>
/// Constants for voucher posting status
/// </summary>
public static class VoucherPostingStatus
{
    /// <summary>
    /// Voucher has been posted to the ledger and is active
    /// </summary>
    public const string Posted = "Posted";

    /// <summary>
    /// Voucher has been cancelled (reversed)
    /// </summary>
    public const string Cancelled = "Cancelled";

    /// <summary>
    /// Voucher is in draft state (not yet posted)
    /// </summary>
    public const string Draft = "Draft";

    /// <summary>
    /// Voucher posting failed
    /// </summary>
    public const string Failed = "Failed";
}
