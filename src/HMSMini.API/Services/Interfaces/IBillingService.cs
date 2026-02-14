using HMSMini.API.Models.DTOs.Billing;

namespace HMSMini.API.Services.Interfaces;

/// <summary>
/// Service for billing operations and invoice management
/// </summary>
public interface IBillingService
{
    /// <summary>
    /// Get bill preview for a check-in before finalizing checkout
    /// </summary>
    Task<BillPreviewDto> GetBillPreviewAsync(int checkInId, DateTime? proposedCheckOutDate = null);

    /// <summary>
    /// Finalize checkout and create invoice
    /// </summary>
    Task<InvoiceDto> FinalizeInvoiceAsync(int checkInId, FinalizeInvoiceDto dto);

    /// <summary>
    /// Get invoice by invoice ID
    /// </summary>
    Task<InvoiceDto?> GetInvoiceByIdAsync(int invoiceId);

    /// <summary>
    /// Get invoice by check-in ID
    /// </summary>
    Task<InvoiceDto?> GetInvoiceByCheckInIdAsync(int checkInId);

    /// <summary>
    /// Add additional charge to a check-in
    /// </summary>
    Task<AdditionalChargeDto> AddChargeAsync(int checkInId, CreateAdditionalChargeDto dto, string? addedBy = null);

    /// <summary>
    /// Get all additional charges for a check-in
    /// </summary>
    Task<List<AdditionalChargeDto>> GetChargesByCheckInAsync(int checkInId);

    /// <summary>
    /// Update an additional charge
    /// </summary>
    Task<AdditionalChargeDto?> UpdateChargeAsync(int chargeId, UpdateAdditionalChargeDto dto);

    /// <summary>
    /// Delete (soft delete) an additional charge
    /// </summary>
    Task<bool> DeleteChargeAsync(int chargeId, string? deletedBy = null);
}
