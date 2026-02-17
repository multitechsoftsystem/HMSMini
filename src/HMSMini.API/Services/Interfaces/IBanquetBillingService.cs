using HMSMini.API.Models.DTOs.BanquetBilling;

namespace HMSMini.API.Services.Interfaces;

public interface IBanquetBillingService
{
    Task<BanquetBillPreviewDto> GetBillPreviewAsync(int bookingId);
    Task<BanquetInvoiceDto> FinalizeInvoiceAsync(int bookingId, FinalizeBanquetInvoiceDto dto);
    Task<BanquetInvoiceDto?> GetInvoiceByIdAsync(int invoiceId);
    Task<BanquetInvoiceDto?> GetInvoiceByBookingIdAsync(int bookingId);
    Task<List<BanquetInvoiceListDto>> GetAllInvoicesAsync();
}
