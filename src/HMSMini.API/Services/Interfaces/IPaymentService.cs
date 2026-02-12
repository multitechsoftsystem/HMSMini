using HMSMini.API.Models.DTOs.Payment;

namespace HMSMini.API.Services.Interfaces;

public interface IPaymentService
{
    Task<PaymentDto> CreateAsync(CreatePaymentDto dto);
    Task<PaymentDto?> GetByIdAsync(int id);
    Task<List<PaymentDto>> GetByCheckInIdAsync(int checkInId);
    Task<List<PaymentDto>> GetByBanquetBookingIdAsync(int bookingId);
    Task<PaymentSummaryDto> GetPaymentSummaryForCheckInAsync(int checkInId);
    Task<PaymentSummaryDto> GetPaymentSummaryForBanquetAsync(int bookingId);
    Task<List<PaymentDto>> GetByCompanyIdAsync(int companyId, DateTime? fromDate = null, DateTime? toDate = null);
    Task<PaymentDto> CancelPaymentAsync(int id, string? reason, string? cancelledBy = null);
}
