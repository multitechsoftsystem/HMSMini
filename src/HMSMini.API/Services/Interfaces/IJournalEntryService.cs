using HMSMini.API.Models.DTOs.JournalEntry;
using HMSMini.API.Models.Enums;

namespace HMSMini.API.Services.Interfaces;

public interface IJournalEntryService
{
    Task<JournalEntryDto> GetByIdAsync(int id);
    Task<List<JournalEntryDto>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int? financialYearId = null);
    Task<JournalEntryDto> CreateAsync(CreateJournalEntryDto dto, string? createdBy = null);
    Task<JournalEntryDto> CreateReversalAsync(int id, string? createdBy = null);
    Task<int> PostJournalEntryAsync(DateTime entryDate, string description, JournalSourceType sourceType, int? sourceId, List<(int accountId, decimal debit, decimal credit, string? desc)> lines, string? createdBy = null);
}
