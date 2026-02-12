using HMSMini.API.Models.DTOs.BanquetPayment;

namespace HMSMini.API.Services.Interfaces;

[Obsolete("Use IPaymentService instead. This service will be removed in a future version.")]
public interface IBanquetPaymentService
{
    Task<List<BanquetPaymentDto>> GetByBookingAsync(int bookingId);
    Task<BanquetPaymentDto> CreateAsync(int bookingId, CreateBanquetPaymentDto dto);
    Task<BanquetPaymentSummaryDto> GetPaymentSummaryAsync(int bookingId);
}
