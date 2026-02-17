using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.JournalEntry;
using HMSMini.API.Models.Entities;
using HMSMini.API.Models.Enums;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Services.Implementations;

public class JournalEntryService : IJournalEntryService
{
    private readonly ApplicationDbContext _context;
    private readonly IFinancialYearService _financialYearService;
    private readonly ILogger<JournalEntryService> _logger;

    public JournalEntryService(
        ApplicationDbContext context,
        IFinancialYearService financialYearService,
        ILogger<JournalEntryService> logger)
    {
        _context = context;
        _financialYearService = financialYearService;
        _logger = logger;
    }

    public async Task<JournalEntryDto> GetByIdAsync(int id)
    {
        var entry = await _context.JournalEntries
            .Include(j => j.Lines).ThenInclude(l => l.Account)
            .Include(j => j.FinancialYear)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (entry == null)
            throw new NotFoundException(nameof(JournalEntry), id);

        return MapToDto(entry);
    }

    public async Task<List<JournalEntryDto>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int? financialYearId = null)
    {
        var query = _context.JournalEntries
            .Include(j => j.Lines).ThenInclude(l => l.Account)
            .Include(j => j.FinancialYear)
            .AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(j => j.EntryDate >= fromDate.Value.Date);

        if (toDate.HasValue)
            query = query.Where(j => j.EntryDate <= toDate.Value.Date);

        if (financialYearId.HasValue)
            query = query.Where(j => j.FinancialYearId == financialYearId.Value);

        return await query
            .OrderByDescending(j => j.EntryDate)
            .ThenByDescending(j => j.Id)
            .AsNoTracking()
            .Select(j => MapToDto(j))
            .ToListAsync();
    }

    public async Task<JournalEntryDto> CreateAsync(CreateJournalEntryDto dto, string? createdBy = null)
    {
        // Validate lines balance
        var totalDebit = dto.Lines.Sum(l => l.DebitAmount);
        var totalCredit = dto.Lines.Sum(l => l.CreditAmount);

        if (totalDebit != totalCredit)
            throw new BusinessRuleException($"Journal entry is not balanced. Debit: {totalDebit}, Credit: {totalCredit}");

        if (totalDebit == 0)
            throw new BusinessRuleException("Journal entry total cannot be zero.");

        var lines = dto.Lines.Select(l => (l.AccountId, l.DebitAmount, l.CreditAmount, l.Description)).ToList();

        var entryId = await PostJournalEntryAsync(
            dto.EntryDate,
            dto.Description ?? "Manual journal entry",
            JournalSourceType.Manual,
            null,
            lines,
            createdBy);

        return await GetByIdAsync(entryId);
    }

    public async Task<JournalEntryDto> CreateReversalAsync(int id, string? createdBy = null)
    {
        var original = await _context.JournalEntries
            .Include(j => j.Lines)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (original == null)
            throw new NotFoundException(nameof(JournalEntry), id);

        if (original.IsReversed)
            throw new BusinessRuleException("This journal entry has already been reversed.");

        // Create reversal lines (swap debit/credit)
        var reversalLines = original.Lines
            .Select(l => (l.AccountId, l.CreditAmount, l.DebitAmount, (string?)$"Reversal of {original.EntryNumber}"))
            .ToList();

        var reversalId = await PostJournalEntryAsync(
            DateTime.Today,
            $"Reversal of {original.EntryNumber}",
            original.SourceType,
            original.SourceId,
            reversalLines,
            createdBy);

        // Mark original as reversed
        original.IsReversed = true;
        var reversalEntry = await _context.JournalEntries.FindAsync(reversalId);
        if (reversalEntry != null)
            reversalEntry.ReversalOfId = original.Id;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Reversed journal entry {EntryNumber}", original.EntryNumber);
        return await GetByIdAsync(reversalId);
    }

    public async Task<int> PostJournalEntryAsync(
        DateTime entryDate,
        string description,
        JournalSourceType sourceType,
        int? sourceId,
        List<(int accountId, decimal debit, decimal credit, string? desc)> lines,
        string? createdBy = null)
    {
        // Get current financial year
        var fyId = await _financialYearService.GetCurrentFinancialYearIdAsync();

        // Validate FY not closed
        var fy = await _context.FinancialYears.FindAsync(fyId);
        if (fy != null && fy.IsClosed)
            throw new BusinessRuleException("Cannot post to a closed financial year.");

        // Validate debits == credits
        var totalDebit = lines.Sum(l => l.debit);
        var totalCredit = lines.Sum(l => l.credit);

        if (totalDebit != totalCredit)
            throw new BusinessRuleException($"Journal entry is not balanced. Debit: {totalDebit}, Credit: {totalCredit}");

        // Generate entry number: JE-YYYYMMDD-NNNN
        var entryNumber = await GenerateEntryNumberAsync(entryDate);

        var journalEntry = new JournalEntry
        {
            EntryNumber = entryNumber,
            EntryDate = entryDate,
            FinancialYearId = fyId,
            Description = description,
            SourceType = sourceType,
            SourceId = sourceId,
            TotalAmount = totalDebit,
            CreatedBy = createdBy
        };

        foreach (var line in lines)
        {
            if (line.debit == 0 && line.credit == 0)
                continue;

            journalEntry.Lines.Add(new JournalEntryLine
            {
                AccountId = line.accountId,
                DebitAmount = line.debit,
                CreditAmount = line.credit,
                Description = line.desc
            });
        }

        _context.JournalEntries.Add(journalEntry);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Posted journal entry {EntryNumber}: {Description}, Amount: {Amount}",
            entryNumber, description, totalDebit);

        return journalEntry.Id;
    }

    private async Task<string> GenerateEntryNumberAsync(DateTime date)
    {
        var prefix = $"JE-{date:yyyyMMdd}-";

        var lastEntry = await _context.JournalEntries
            .IgnoreQueryFilters()
            .Where(j => j.EntryNumber.StartsWith(prefix))
            .OrderByDescending(j => j.Id)
            .FirstOrDefaultAsync();

        int nextNumber = 1;
        if (lastEntry != null)
        {
            var lastNumberStr = lastEntry.EntryNumber.Substring(prefix.Length);
            if (int.TryParse(lastNumberStr, out int lastNumber))
                nextNumber = lastNumber + 1;
        }

        return $"{prefix}{nextNumber:D4}";
    }

    private static JournalEntryDto MapToDto(JournalEntry entry)
    {
        return new JournalEntryDto
        {
            Id = entry.Id,
            EntryNumber = entry.EntryNumber,
            EntryDate = entry.EntryDate,
            FinancialYearId = entry.FinancialYearId,
            FinancialYearName = entry.FinancialYear?.Name,
            Description = entry.Description,
            SourceType = entry.SourceType,
            SourceId = entry.SourceId,
            TotalAmount = entry.TotalAmount,
            IsReversed = entry.IsReversed,
            ReversalOfId = entry.ReversalOfId,
            CreatedAt = entry.CreatedAt,
            CreatedBy = entry.CreatedBy,
            Lines = entry.Lines.Select(l => new JournalEntryLineDto
            {
                Id = l.Id,
                AccountId = l.AccountId,
                AccountCode = l.Account?.AccountCode ?? "",
                AccountName = l.Account?.AccountName ?? "",
                DebitAmount = l.DebitAmount,
                CreditAmount = l.CreditAmount,
                Description = l.Description
            }).ToList()
        };
    }
}
