using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.FinancialYear;
using HMSMini.API.Models.Entities;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Services.Implementations;

public class FinancialYearService : IFinancialYearService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<FinancialYearService> _logger;

    public FinancialYearService(ApplicationDbContext context, ILogger<FinancialYearService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<FinancialYearDto>> GetAllAsync()
    {
        return await _context.FinancialYears
            .OrderByDescending(f => f.StartDate)
            .Select(f => MapToDto(f))
            .ToListAsync();
    }

    public async Task<FinancialYearDto> GetByIdAsync(int id)
    {
        var fy = await _context.FinancialYears.FindAsync(id);
        if (fy == null)
            throw new NotFoundException(nameof(FinancialYear), id);
        return MapToDto(fy);
    }

    public async Task<FinancialYearDto?> GetCurrentAsync()
    {
        var fy = await _context.FinancialYears.FirstOrDefaultAsync(f => f.IsCurrent);
        return fy == null ? null : MapToDto(fy);
    }

    public async Task<FinancialYearDto> CreateAsync(CreateFinancialYearDto dto)
    {
        if (await _context.FinancialYears.AnyAsync(f => f.Name == dto.Name))
            throw new BusinessRuleException($"Financial year '{dto.Name}' already exists.");

        var fy = new FinancialYear
        {
            Name = dto.Name,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsCurrent = false,
            IsClosed = false,
            CreatedBy = "System"
        };

        _context.FinancialYears.Add(fy);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created financial year {Name}", fy.Name);
        return MapToDto(fy);
    }

    public async Task SetCurrentAsync(int id)
    {
        var fy = await _context.FinancialYears.FindAsync(id);
        if (fy == null)
            throw new NotFoundException(nameof(FinancialYear), id);

        if (fy.IsClosed)
            throw new BusinessRuleException("Cannot set a closed financial year as current.");

        // Unset all others
        var currentFys = await _context.FinancialYears.Where(f => f.IsCurrent).ToListAsync();
        foreach (var current in currentFys)
        {
            current.IsCurrent = false;
        }

        fy.IsCurrent = true;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Set financial year {Name} as current", fy.Name);
    }

    public async Task CloseAsync(int id, string? closedBy = null)
    {
        var fy = await _context.FinancialYears.FindAsync(id);
        if (fy == null)
            throw new NotFoundException(nameof(FinancialYear), id);

        if (fy.IsClosed)
            throw new BusinessRuleException("Financial year is already closed.");

        fy.IsClosed = true;
        fy.IsCurrent = false;
        fy.ClosedAt = DateTime.UtcNow;
        fy.ClosedBy = closedBy;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Closed financial year {Name} by {User}", fy.Name, closedBy);
    }

    public async Task<int> GetCurrentFinancialYearIdAsync()
    {
        var fy = await _context.FinancialYears.FirstOrDefaultAsync(f => f.IsCurrent);
        if (fy == null)
            throw new BusinessRuleException("No current financial year is set. Please configure a financial year.");
        return fy.Id;
    }

    private static FinancialYearDto MapToDto(FinancialYear fy)
    {
        return new FinancialYearDto
        {
            Id = fy.Id,
            Name = fy.Name,
            StartDate = fy.StartDate,
            EndDate = fy.EndDate,
            IsCurrent = fy.IsCurrent,
            IsClosed = fy.IsClosed,
            ClosedAt = fy.ClosedAt,
            ClosedBy = fy.ClosedBy,
            CreatedAt = fy.CreatedAt
        };
    }
}
