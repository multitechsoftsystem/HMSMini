namespace HMSMini.API.Models.Enums;

/// <summary>
/// Constants for voucher types in the accounting ledger
/// </summary>
public static class VoucherType
{
    /// <summary>
    /// Daily room rent charge
    /// </summary>
    public const string RoomTariff = "RoomTariff";

    /// <summary>
    /// Meal plan charge (breakfast, half board, full board, etc.)
    /// </summary>
    public const string MealPlan = "MealPlan";

    /// <summary>
    /// Central Goods and Services Tax
    /// </summary>
    public const string TaxCGST = "Tax-CGST";

    /// <summary>
    /// State Goods and Services Tax
    /// </summary>
    public const string TaxSGST = "Tax-SGST";

    /// <summary>
    /// Integrated Goods and Services Tax
    /// </summary>
    public const string TaxIGST = "Tax-IGST";

    /// <summary>
    /// Additional charges like minibar, laundry, room service, etc.
    /// </summary>
    public const string AdditionalCharge = "AdditionalCharge";

    /// <summary>
    /// Discount voucher (negative amount)
    /// </summary>
    public const string Discount = "Discount";

    /// <summary>
    /// Reversing voucher to cancel a previous entry (negative amount)
    /// </summary>
    public const string Reversal = "Reversal";

    /// <summary>
    /// Payment received from guest
    /// </summary>
    public const string Payment = "Payment";

    /// <summary>
    /// Refund to guest
    /// </summary>
    public const string Refund = "Refund";

    // Banquet voucher types

    /// <summary>
    /// Banquet hall rent charge
    /// </summary>
    public const string BanquetHallRent = "BanquetHallRent";

    /// <summary>
    /// Banquet menu/food charge
    /// </summary>
    public const string BanquetMenuCharge = "BanquetMenuCharge";

    /// <summary>
    /// Banquet service charge
    /// </summary>
    public const string BanquetServiceCharge = "BanquetServiceCharge";

    /// <summary>
    /// Banquet additional/ad-hoc charge
    /// </summary>
    public const string BanquetAdditionalCharge = "BanquetAdditionalCharge";

    /// <summary>
    /// Banquet payment received
    /// </summary>
    public const string BanquetPayment = "BanquetPayment";

    /// <summary>
    /// Banquet refund
    /// </summary>
    public const string BanquetRefund = "BanquetRefund";
}
