using HMSMini.API.Models.DTOs.Receipt;

namespace HMSMini.API.Services.Interfaces;

public interface IReceiptService
{
    Task<ReceiptDto> GetByIdAsync(int id);
    Task<List<ReceiptListDto>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate);
    Task<List<OutstandingInvoiceDto>> GetOutstandingInvoicesAsync(int? companyId = null);
    Task<ReceiptDto> CreateAsync(CreateReceiptDto dto, string? createdBy = null);
}
