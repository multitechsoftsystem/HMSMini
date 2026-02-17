using HMSMini.API.Models.DTOs.ChartOfAccount;
using HMSMini.API.Models.Enums;

namespace HMSMini.API.Services.Interfaces;

public interface IChartOfAccountService
{
    Task<List<ChartOfAccountDto>> GetAllAsync();
    Task<ChartOfAccountDto> GetByIdAsync(int id);
    Task<List<AccountDropdownDto>> GetDropdownAsync(AccountType? type = null);
    Task<ChartOfAccountDto> CreateAsync(CreateChartOfAccountDto dto);
    Task<ChartOfAccountDto> UpdateAsync(int id, UpdateChartOfAccountDto dto);
    Task DeleteAsync(int id);
    Task<int> GetAccountIdByCodeAsync(string code);
}
