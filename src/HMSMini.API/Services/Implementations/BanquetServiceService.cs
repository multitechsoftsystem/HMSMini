using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.BanquetService;
using HMSMini.API.Models.Entities;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Services.Implementations;

public class BanquetServiceService : IBanquetServiceService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BanquetServiceService> _logger;

    public BanquetServiceService(ApplicationDbContext context, ILogger<BanquetServiceService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<BanquetServiceDto>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.BanquetServices.Where(s => s.DeletedAt == null);
        if (!includeInactive)
            query = query.Where(s => s.IsActive);

        return await query.Select(s => new BanquetServiceDto
        {
            Id = s.Id,
            ServiceName = s.ServiceName,
            DefaultRate = s.DefaultRate,
            Unit = s.Unit,
            ApplyTax = s.ApplyTax,
            VoucherTaxConfigId = s.VoucherTaxConfigId,
            IsActive = s.IsActive,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt
        }).OrderBy(s => s.ServiceName).ToListAsync();
    }

    public async Task<List<BanquetServiceDto>> GetActiveAsync()
    {
        return await _context.BanquetServices
            .Where(s => s.IsActive && s.DeletedAt == null)
            .Select(s => new BanquetServiceDto
            {
                Id = s.Id,
                ServiceName = s.ServiceName,
                DefaultRate = s.DefaultRate,
                Unit = s.Unit,
                ApplyTax = s.ApplyTax,
                VoucherTaxConfigId = s.VoucherTaxConfigId,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            }).OrderBy(s => s.ServiceName).ToListAsync();
    }

    public async Task<BanquetServiceDto?> GetByIdAsync(int id)
    {
        return await _context.BanquetServices
            .Where(s => s.Id == id && s.DeletedAt == null)
            .Select(s => new BanquetServiceDto
            {
                Id = s.Id,
                ServiceName = s.ServiceName,
                DefaultRate = s.DefaultRate,
                Unit = s.Unit,
                ApplyTax = s.ApplyTax,
                VoucherTaxConfigId = s.VoucherTaxConfigId,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            }).FirstOrDefaultAsync();
    }

    public async Task<BanquetServiceDto> CreateAsync(CreateBanquetServiceDto dto)
    {
        var entity = new MBanquetService
        {
            ServiceName = dto.ServiceName,
            DefaultRate = dto.DefaultRate,
            Unit = dto.Unit,
            ApplyTax = dto.ApplyTax,
            VoucherTaxConfigId = dto.VoucherTaxConfigId,
            IsActive = true
        };

        _context.BanquetServices.Add(entity);
        await _context.SaveChangesAsync();
        return (await GetByIdAsync(entity.Id))!;
    }

    public async Task<BanquetServiceDto> UpdateAsync(int id, UpdateBanquetServiceDto dto)
    {
        var entity = await _context.BanquetServices.FindAsync(id);
        if (entity == null || entity.DeletedAt != null)
            throw new NotFoundException(nameof(MBanquetService), id);

        entity.ServiceName = dto.ServiceName;
        entity.DefaultRate = dto.DefaultRate;
        entity.Unit = dto.Unit;
        entity.ApplyTax = dto.ApplyTax;
        entity.VoucherTaxConfigId = dto.VoucherTaxConfigId;

        await _context.SaveChangesAsync();
        return (await GetByIdAsync(id))!;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.BanquetServices.FindAsync(id);
        if (entity == null || entity.DeletedAt != null)
            throw new NotFoundException(nameof(MBanquetService), id);

        entity.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<BanquetServiceDto> ActivateAsync(int id)
    {
        var entity = await _context.BanquetServices.FindAsync(id);
        if (entity == null || entity.DeletedAt != null)
            throw new NotFoundException(nameof(MBanquetService), id);

        entity.IsActive = true;
        await _context.SaveChangesAsync();
        return (await GetByIdAsync(id))!;
    }

    public async Task<BanquetServiceDto> DeactivateAsync(int id)
    {
        var entity = await _context.BanquetServices.FindAsync(id);
        if (entity == null || entity.DeletedAt != null)
            throw new NotFoundException(nameof(MBanquetService), id);

        entity.IsActive = false;
        await _context.SaveChangesAsync();
        return (await GetByIdAsync(id))!;
    }
}
