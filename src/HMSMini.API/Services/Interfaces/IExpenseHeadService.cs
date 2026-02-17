using HMSMini.API.Models.DTOs.ExpenseHead;

namespace HMSMini.API.Services.Interfaces;

public interface IExpenseHeadService
{
    Task<List<ExpenseHeadDto>> GetAllAsync();
    Task<ExpenseHeadDto> GetByIdAsync(int id);
    Task<ExpenseHeadDto> CreateAsync(CreateExpenseHeadDto dto);
    Task<ExpenseHeadDto> UpdateAsync(int id, UpdateExpenseHeadDto dto);
    Task DeleteAsync(int id);
}
