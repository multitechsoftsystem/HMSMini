using HMSMini.API.Models.DTOs.PaymentVoucher;

namespace HMSMini.API.Services.Interfaces;

public interface IPaymentVoucherService
{
    Task<PaymentVoucherDto> GetByIdAsync(int id);
    Task<List<PaymentVoucherListDto>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate);
    Task<PaymentVoucherDto> CreateAsync(CreatePaymentVoucherDto dto, string? createdBy = null);
}
