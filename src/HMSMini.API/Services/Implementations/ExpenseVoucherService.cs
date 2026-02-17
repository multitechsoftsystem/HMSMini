using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.ExpenseVoucher;
using HMSMini.API.Models.Entities;
using HMSMini.API.Models.Enums;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Services.Implementations;

public class ExpenseVoucherService : IExpenseVoucherService
{
    private readonly ApplicationDbContext _context;
    private readonly IFinancialYearService _financialYearService;
    private readonly IJournalEntryService _journalEntryService;
    private readonly IChartOfAccountService _chartOfAccountService;
    private readonly ILogger<ExpenseVoucherService> _logger;

    public ExpenseVoucherService(
        ApplicationDbContext context,
        IFinancialYearService financialYearService,
        IJournalEntryService journalEntryService,
        IChartOfAccountService chartOfAccountService,
        ILogger<ExpenseVoucherService> logger)
    {
        _context = context;
        _financialYearService = financialYearService;
        _journalEntryService = journalEntryService;
        _chartOfAccountService = chartOfAccountService;
        _logger = logger;
    }

    public async Task<ExpenseVoucherDto> GetByIdAsync(int id)
    {
        var voucher = await _context.ExpenseVouchers
            .Include(v => v.ExpenseHead)
            .Include(v => v.BankAccount)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (voucher == null)
            throw new NotFoundException(nameof(ExpenseVoucher), id);

        return MapToDto(voucher);
    }

    public async Task<List<ExpenseVoucherListDto>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate)
    {
        var query = _context.ExpenseVouchers
            .Include(v => v.ExpenseHead)
            .AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(v => v.VoucherDate >= fromDate.Value.Date);

        if (toDate.HasValue)
            query = query.Where(v => v.VoucherDate <= toDate.Value.Date);

        return await query
            .OrderByDescending(v => v.VoucherDate)
            .ThenByDescending(v => v.Id)
            .Select(v => new ExpenseVoucherListDto
            {
                Id = v.Id,
                VoucherNumber = v.VoucherNumber,
                VoucherDate = v.VoucherDate,
                ExpenseHeadName = v.ExpenseHead.Name,
                Amount = v.Amount,
                PaidTo = v.PaidTo,
                PaymentModeName = v.PaymentMode.ToString(),
                CreatedAt = v.CreatedAt
            })
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ExpenseVoucherDto> CreateAsync(CreateExpenseVoucherDto dto, string? createdBy = null)
    {
        if (dto.Amount <= 0)
            throw new BusinessRuleException("Amount must be greater than zero.");

        var fyId = await _financialYearService.GetCurrentFinancialYearIdAsync();

        // Get expense head to find the GL account
        var expenseHead = await _context.ExpenseHeads.FindAsync(dto.ExpenseHeadId);
        if (expenseHead == null)
            throw new NotFoundException(nameof(MExpenseHead), dto.ExpenseHeadId);

        // Determine debit account (expense GL)
        int debitAccountId;
        if (expenseHead.DefaultAccountId.HasValue)
        {
            debitAccountId = expenseHead.DefaultAccountId.Value;
        }
        else
        {
            debitAccountId = await _chartOfAccountService.GetAccountIdByCodeAsync("5099"); // Misc expense
        }

        // Determine credit account (Cash or Bank)
        int creditAccountId;
        if (dto.PaymentMode == PaymentMode.Cash)
        {
            creditAccountId = await _chartOfAccountService.GetAccountIdByCodeAsync("1001"); // Cash
        }
        else
        {
            creditAccountId = dto.BankAccountId ?? await _chartOfAccountService.GetAccountIdByCodeAsync("1002"); // Bank
        }

        // Generate voucher number
        var voucherNumber = await GenerateVoucherNumberAsync(dto.VoucherDate);

        var voucher = new ExpenseVoucher
        {
            VoucherNumber = voucherNumber,
            VoucherDate = dto.VoucherDate,
            FinancialYearId = fyId,
            ExpenseHeadId = dto.ExpenseHeadId,
            Amount = dto.Amount,
            PaidTo = dto.PaidTo,
            PaymentMode = dto.PaymentMode,
            BankAccountId = dto.BankAccountId,
            ReferenceNumber = dto.ReferenceNumber,
            Narration = dto.Narration,
            CreatedBy = createdBy
        };

        _context.ExpenseVouchers.Add(voucher);
        await _context.SaveChangesAsync();

        // Create journal entry: Dr. Expense / Cr. Cash or Bank
        try
        {
            var lines = new List<(int accountId, decimal debit, decimal credit, string? desc)>
            {
                (debitAccountId, dto.Amount, 0, $"{expenseHead.Name} - {dto.PaidTo}"),
                (creditAccountId, 0, dto.Amount, $"Payment for {expenseHead.Name}")
            };

            var jeId = await _journalEntryService.PostJournalEntryAsync(
                dto.VoucherDate,
                $"Expense Voucher {voucherNumber}: {expenseHead.Name}",
                JournalSourceType.ExpenseVoucher,
                voucher.Id,
                lines,
                createdBy);

            voucher.JournalEntryId = jeId;
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to create journal entry for expense voucher {VoucherNumber}", voucherNumber);
        }

        _logger.LogInformation("Created expense voucher {VoucherNumber} for {Amount}", voucherNumber, dto.Amount);
        return await GetByIdAsync(voucher.Id);
    }

    private async Task<string> GenerateVoucherNumberAsync(DateTime date)
    {
        var prefix = $"EXP-{date:yyyyMMdd}-";

        var last = await _context.ExpenseVouchers
            .IgnoreQueryFilters()
            .Where(v => v.VoucherNumber.StartsWith(prefix))
            .OrderByDescending(v => v.Id)
            .FirstOrDefaultAsync();

        int nextNumber = 1;
        if (last != null)
        {
            var lastStr = last.VoucherNumber.Substring(prefix.Length);
            if (int.TryParse(lastStr, out int lastNumber))
                nextNumber = lastNumber + 1;
        }

        return $"{prefix}{nextNumber:D4}";
    }

    private static ExpenseVoucherDto MapToDto(ExpenseVoucher v)
    {
        return new ExpenseVoucherDto
        {
            Id = v.Id,
            VoucherNumber = v.VoucherNumber,
            VoucherDate = v.VoucherDate,
            FinancialYearId = v.FinancialYearId,
            ExpenseHeadId = v.ExpenseHeadId,
            ExpenseHeadName = v.ExpenseHead?.Name ?? "",
            Amount = v.Amount,
            PaidTo = v.PaidTo,
            PaymentMode = v.PaymentMode,
            BankAccountId = v.BankAccountId,
            BankAccountName = v.BankAccount != null ? $"{v.BankAccount.AccountCode} - {v.BankAccount.AccountName}" : null,
            ReferenceNumber = v.ReferenceNumber,
            Narration = v.Narration,
            JournalEntryId = v.JournalEntryId,
            CreatedAt = v.CreatedAt,
            CreatedBy = v.CreatedBy
        };
    }
}
