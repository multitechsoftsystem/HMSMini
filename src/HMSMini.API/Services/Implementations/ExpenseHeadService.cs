using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.ExpenseHead;
using HMSMini.API.Models.Entities;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Services.Implementations;

public class ExpenseHeadService : IExpenseHeadService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ExpenseHeadService> _logger;

    public ExpenseHeadService(ApplicationDbContext context, ILogger<ExpenseHeadService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<ExpenseHeadDto>> GetAllAsync()
    {
        return await _context.ExpenseHeads
            .Include(e => e.DefaultAccount)
            .OrderBy(e => e.Name)
            .Select(e => new ExpenseHeadDto
            {
                Id = e.Id,
                Name = e.Name,
                DefaultAccountId = e.DefaultAccountId,
                DefaultAccountName = e.DefaultAccount != null ? $"{e.DefaultAccount.AccountCode} - {e.DefaultAccount.AccountName}" : null,
                IsActive = e.IsActive
            })
            .ToListAsync();
    }

    public async Task<ExpenseHeadDto> GetByIdAsync(int id)
    {
        var head = await _context.ExpenseHeads
            .Include(e => e.DefaultAccount)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (head == null)
            throw new NotFoundException(nameof(MExpenseHead), id);

        return new ExpenseHeadDto
        {
            Id = head.Id,
            Name = head.Name,
            DefaultAccountId = head.DefaultAccountId,
            DefaultAccountName = head.DefaultAccount != null ? $"{head.DefaultAccount.AccountCode} - {head.DefaultAccount.AccountName}" : null,
            IsActive = head.IsActive
        };
    }

    public async Task<ExpenseHeadDto> CreateAsync(CreateExpenseHeadDto dto)
    {
        if (await _context.ExpenseHeads.AnyAsync(e => e.Name == dto.Name))
            throw new BusinessRuleException($"Expense head '{dto.Name}' already exists.");

        var head = new MExpenseHead
        {
            Name = dto.Name,
            DefaultAccountId = dto.DefaultAccountId,
            IsActive = true
        };

        _context.ExpenseHeads.Add(head);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created expense head {Name}", head.Name);
        return await GetByIdAsync(head.Id);
    }

    public async Task<ExpenseHeadDto> UpdateAsync(int id, UpdateExpenseHeadDto dto)
    {
        var head = await _context.ExpenseHeads.FindAsync(id);
        if (head == null)
            throw new NotFoundException(nameof(MExpenseHead), id);

        if (await _context.ExpenseHeads.AnyAsync(e => e.Name == dto.Name && e.Id != id))
            throw new BusinessRuleException($"Expense head '{dto.Name}' already exists.");

        head.Name = dto.Name;
        head.DefaultAccountId = dto.DefaultAccountId;
        head.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated expense head {Name}", head.Name);
        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        var head = await _context.ExpenseHeads.FindAsync(id);
        if (head == null)
            throw new NotFoundException(nameof(MExpenseHead), id);

        var hasVouchers = await _context.ExpenseVouchers.AnyAsync(v => v.ExpenseHeadId == id);
        if (hasVouchers)
            throw new BusinessRuleException("Cannot delete expense head with existing vouchers.");

        head.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted expense head {Name}", head.Name);
    }
}
