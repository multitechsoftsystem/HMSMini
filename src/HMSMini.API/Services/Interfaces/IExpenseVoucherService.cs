using HMSMini.API.Models.DTOs.ExpenseVoucher;

namespace HMSMini.API.Services.Interfaces;

public interface IExpenseVoucherService
{
    Task<ExpenseVoucherDto> GetByIdAsync(int id);
    Task<List<ExpenseVoucherListDto>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate);
    Task<ExpenseVoucherDto> CreateAsync(CreateExpenseVoucherDto dto, string? createdBy = null);
}
