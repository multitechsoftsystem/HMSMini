using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Models.DTOs.AccountingReport;
using HMSMini.API.Models.Enums;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Services.Implementations;

public class AccountingReportService : IAccountingReportService
{
    private readonly ApplicationDbContext _context;
    private readonly IFinancialYearService _financialYearService;
    private readonly ILogger<AccountingReportService> _logger;

    public AccountingReportService(
        ApplicationDbContext context,
        IFinancialYearService financialYearService,
        ILogger<AccountingReportService> logger)
    {
        _context = context;
        _financialYearService = financialYearService;
        _logger = logger;
    }

    public async Task<TrialBalanceDto> GetTrialBalanceAsync(int? financialYearId = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var result = new TrialBalanceDto
        {
            FinancialYearId = financialYearId,
            FromDate = fromDate,
            ToDate = toDate
        };

        // Get FY details if provided
        if (financialYearId.HasValue)
        {
            var fy = await _context.FinancialYears.FindAsync(financialYearId.Value);
            if (fy != null)
            {
                result.FinancialYearName = fy.Name;
                fromDate ??= fy.StartDate;
                toDate ??= fy.EndDate;
            }
        }

        // Get all active accounts
        var accounts = await _context.ChartOfAccounts
            .Where(a => a.IsActive)
            .OrderBy(a => a.AccountCode)
            .AsNoTracking()
            .ToListAsync();

        // Get journal entry lines within date range
        var linesQuery = _context.JournalEntryLines
            .Include(l => l.JournalEntry)
            .Where(l => l.JournalEntry.DeletedAt == null);

        if (financialYearId.HasValue)
            linesQuery = linesQuery.Where(l => l.JournalEntry.FinancialYearId == financialYearId.Value);

        // Opening balances (before fromDate)
        var openingLines = new Dictionary<int, (decimal debit, decimal credit)>();
        if (fromDate.HasValue)
        {
            var openingData = await _context.JournalEntryLines
                .Include(l => l.JournalEntry)
                .Where(l => l.JournalEntry.DeletedAt == null)
                .Where(l => l.JournalEntry.EntryDate < fromDate.Value)
                .GroupBy(l => l.AccountId)
                .Select(g => new
                {
                    AccountId = g.Key,
                    TotalDebit = g.Sum(l => l.DebitAmount),
                    TotalCredit = g.Sum(l => l.CreditAmount)
                })
                .ToListAsync();

            foreach (var item in openingData)
                openingLines[item.AccountId] = (item.TotalDebit, item.TotalCredit);
        }

        // Period balances
        var periodQuery = linesQuery;
        if (fromDate.HasValue)
            periodQuery = periodQuery.Where(l => l.JournalEntry.EntryDate >= fromDate.Value);
        if (toDate.HasValue)
            periodQuery = periodQuery.Where(l => l.JournalEntry.EntryDate <= toDate.Value);

        var periodData = await periodQuery
            .GroupBy(l => l.AccountId)
            .Select(g => new
            {
                AccountId = g.Key,
                TotalDebit = g.Sum(l => l.DebitAmount),
                TotalCredit = g.Sum(l => l.CreditAmount)
            })
            .ToListAsync();

        var periodLines = periodData.ToDictionary(p => p.AccountId, p => (debit: p.TotalDebit, credit: p.TotalCredit));

        foreach (var account in accounts)
        {
            var opening = openingLines.GetValueOrDefault(account.Id);
            var period = periodLines.GetValueOrDefault(account.Id);

            var openingBalance = opening.debit - opening.credit;
            var periodNet = period.debit - period.credit;
            var closingBalance = openingBalance + periodNet;

            // Skip accounts with no activity
            if (opening == default && period == default)
                continue;

            var line = new TrialBalanceLineDto
            {
                AccountId = account.Id,
                AccountCode = account.AccountCode,
                AccountName = account.AccountName,
                AccountTypeName = account.AccountType.ToString(),
                OpeningDebit = openingBalance > 0 ? openingBalance : 0,
                OpeningCredit = openingBalance < 0 ? Math.Abs(openingBalance) : 0,
                PeriodDebit = period.debit,
                PeriodCredit = period.credit,
                ClosingDebit = closingBalance > 0 ? closingBalance : 0,
                ClosingCredit = closingBalance < 0 ? Math.Abs(closingBalance) : 0
            };

            result.Lines.Add(line);
        }

        result.TotalDebit = result.Lines.Sum(l => l.ClosingDebit);
        result.TotalCredit = result.Lines.Sum(l => l.ClosingCredit);

        return result;
    }

    public async Task<BalanceSheetDto> GetBalanceSheetAsync(DateTime? asOfDate = null, int? financialYearId = null)
    {
        asOfDate ??= DateTime.Today;

        var result = new BalanceSheetDto
        {
            AsOfDate = asOfDate.Value,
            FinancialYearId = financialYearId
        };

        if (financialYearId.HasValue)
        {
            var fy = await _context.FinancialYears.FindAsync(financialYearId.Value);
            result.FinancialYearName = fy?.Name;
        }

        // Get all account balances up to asOfDate
        var balances = await _context.JournalEntryLines
            .Include(l => l.JournalEntry)
            .Include(l => l.Account)
            .Where(l => l.JournalEntry.DeletedAt == null)
            .Where(l => l.JournalEntry.EntryDate <= asOfDate.Value)
            .GroupBy(l => new { l.AccountId, l.Account.AccountCode, l.Account.AccountName, l.Account.AccountType })
            .Select(g => new
            {
                g.Key.AccountId,
                g.Key.AccountCode,
                g.Key.AccountName,
                g.Key.AccountType,
                NetBalance = g.Sum(l => l.DebitAmount) - g.Sum(l => l.CreditAmount)
            })
            .Where(b => b.NetBalance != 0)
            .OrderBy(b => b.AccountCode)
            .ToListAsync();

        decimal totalIncome = 0;
        decimal totalExpense = 0;

        foreach (var balance in balances)
        {
            var line = new BalanceSheetLineDto
            {
                AccountId = balance.AccountId,
                AccountCode = balance.AccountCode,
                AccountName = balance.AccountName,
                Balance = Math.Abs(balance.NetBalance)
            };

            switch (balance.AccountType)
            {
                case AccountType.Asset:
                    // Assets have debit balance (positive)
                    line.Balance = balance.NetBalance;
                    result.Assets.Add(line);
                    break;
                case AccountType.Liability:
                    // Liabilities have credit balance (negative net = positive liability)
                    line.Balance = -balance.NetBalance;
                    result.Liabilities.Add(line);
                    break;
                case AccountType.Equity:
                    line.Balance = -balance.NetBalance;
                    result.Equity.Add(line);
                    break;
                case AccountType.Income:
                    totalIncome += -balance.NetBalance; // Income has credit balance
                    break;
                case AccountType.Expense:
                    totalExpense += balance.NetBalance; // Expense has debit balance
                    break;
            }
        }

        // Calculate retained earnings (Income - Expense)
        result.RetainedEarnings = totalIncome - totalExpense;

        result.TotalAssets = result.Assets.Sum(a => a.Balance);
        result.TotalLiabilities = result.Liabilities.Sum(l => l.Balance);
        result.TotalEquity = result.Equity.Sum(e => e.Balance) + result.RetainedEarnings;

        return result;
    }
}
