using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.ChartOfAccount;
using HMSMini.API.Models.Entities;
using HMSMini.API.Models.Enums;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Services.Implementations;

public class ChartOfAccountService : IChartOfAccountService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ChartOfAccountService> _logger;

    public ChartOfAccountService(ApplicationDbContext context, ILogger<ChartOfAccountService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<ChartOfAccountDto>> GetAllAsync()
    {
        return await _context.ChartOfAccounts
            .Include(c => c.ParentAccount)
            .OrderBy(c => c.AccountCode)
            .Select(c => new ChartOfAccountDto
            {
                Id = c.Id,
                AccountCode = c.AccountCode,
                AccountName = c.AccountName,
                AccountType = c.AccountType,
                ParentAccountId = c.ParentAccountId,
                ParentAccountName = c.ParentAccount != null ? c.ParentAccount.AccountName : null,
                IsSystemAccount = c.IsSystemAccount,
                IsActive = c.IsActive
            })
            .ToListAsync();
    }

    public async Task<ChartOfAccountDto> GetByIdAsync(int id)
    {
        var account = await _context.ChartOfAccounts
            .Include(c => c.ParentAccount)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (account == null)
            throw new NotFoundException(nameof(ChartOfAccount), id);

        return new ChartOfAccountDto
        {
            Id = account.Id,
            AccountCode = account.AccountCode,
            AccountName = account.AccountName,
            AccountType = account.AccountType,
            ParentAccountId = account.ParentAccountId,
            ParentAccountName = account.ParentAccount?.AccountName,
            IsSystemAccount = account.IsSystemAccount,
            IsActive = account.IsActive
        };
    }

    public async Task<List<AccountDropdownDto>> GetDropdownAsync(AccountType? type = null)
    {
        var query = _context.ChartOfAccounts
            .Where(c => c.IsActive);

        if (type.HasValue)
            query = query.Where(c => c.AccountType == type.Value);

        return await query
            .OrderBy(c => c.AccountCode)
            .Select(c => new AccountDropdownDto
            {
                Id = c.Id,
                AccountCode = c.AccountCode,
                AccountName = c.AccountName,
                AccountType = c.AccountType
            })
            .ToListAsync();
    }

    public async Task<ChartOfAccountDto> CreateAsync(CreateChartOfAccountDto dto)
    {
        if (await _context.ChartOfAccounts.AnyAsync(c => c.AccountCode == dto.AccountCode))
            throw new BusinessRuleException($"Account code '{dto.AccountCode}' already exists.");

        var account = new ChartOfAccount
        {
            AccountCode = dto.AccountCode,
            AccountName = dto.AccountName,
            AccountType = dto.AccountType,
            ParentAccountId = dto.ParentAccountId,
            IsSystemAccount = false,
            IsActive = true
        };

        _context.ChartOfAccounts.Add(account);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created account {Code} - {Name}", account.AccountCode, account.AccountName);
        return await GetByIdAsync(account.Id);
    }

    public async Task<ChartOfAccountDto> UpdateAsync(int id, UpdateChartOfAccountDto dto)
    {
        var account = await _context.ChartOfAccounts.FindAsync(id);
        if (account == null)
            throw new NotFoundException(nameof(ChartOfAccount), id);

        if (account.IsSystemAccount)
            throw new BusinessRuleException("Cannot modify a system account.");

        account.AccountName = dto.AccountName;
        account.ParentAccountId = dto.ParentAccountId;
        account.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated account {Code}", account.AccountCode);
        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        var account = await _context.ChartOfAccounts.FindAsync(id);
        if (account == null)
            throw new NotFoundException(nameof(ChartOfAccount), id);

        if (account.IsSystemAccount)
            throw new BusinessRuleException("Cannot delete a system account.");

        // Check if account has journal entry lines
        var hasEntries = await _context.JournalEntryLines.AnyAsync(l => l.AccountId == id);
        if (hasEntries)
            throw new BusinessRuleException("Cannot delete an account that has journal entries.");

        account.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted account {Code}", account.AccountCode);
    }

    public async Task<int> GetAccountIdByCodeAsync(string code)
    {
        var account = await _context.ChartOfAccounts
            .FirstOrDefaultAsync(c => c.AccountCode == code && c.IsActive);

        if (account == null)
            throw new NotFoundException($"Account with code '{code}' not found.");

        return account.Id;
    }
}
