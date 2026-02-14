using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.Company;
using HMSMini.API.Models.Entities;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Services.Implementations;

public class CompanyService : ICompanyService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CompanyService> _logger;

    public CompanyService(ApplicationDbContext context, ILogger<CompanyService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<CompanyListDto>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.Companies
            .Where(c => c.DeletedAt == null);

        if (!includeInactive)
        {
            query = query.Where(c => c.IsActive);
        }

        return await query
            .Select(c => new CompanyListDto
            {
                Id = c.Id,
                CompanyName = c.CompanyName,
                GSTNumber = c.GSTNumber,
                City = c.City,
                ContactPerson = c.ContactPerson,
                ContactNumber = c.ContactNumber,
                DiscountPercentage = c.DiscountPercentage,
                IsActive = c.IsActive,
                TotalCheckIns = c.CheckIns.Count(ci => ci.DeletedAt == null)
            })
            .OrderBy(c => c.CompanyName)
            .ToListAsync();
    }

    public async Task<List<CompanyDto>> GetActiveCompaniesAsync()
    {
        return await _context.Companies
            .Where(c => c.IsActive && c.DeletedAt == null)
            .Select(c => new CompanyDto
            {
                Id = c.Id,
                CompanyName = c.CompanyName,
                Address = c.Address,
                City = c.City,
                State = c.State,
                Country = c.Country,
                GSTNumber = c.GSTNumber,
                ContactPerson = c.ContactPerson,
                Designation = c.Designation,
                Email = c.Email,
                ContactNumber = c.ContactNumber,
                DiscountPercentage = c.DiscountPercentage,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .OrderBy(c => c.CompanyName)
            .ToListAsync();
    }

    public async Task<CompanyDto?> GetByIdAsync(int id)
    {
        var company = await _context.Companies
            .Where(c => c.Id == id && c.DeletedAt == null)
            .Select(c => new CompanyDto
            {
                Id = c.Id,
                CompanyName = c.CompanyName,
                Address = c.Address,
                City = c.City,
                State = c.State,
                Country = c.Country,
                GSTNumber = c.GSTNumber,
                ContactPerson = c.ContactPerson,
                Designation = c.Designation,
                Email = c.Email,
                ContactNumber = c.ContactNumber,
                DiscountPercentage = c.DiscountPercentage,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .FirstOrDefaultAsync();

        return company;
    }

    public async Task<CompanyDto> CreateAsync(CreateCompanyDto dto)
    {
        // Check if GST number already exists (if provided)
        if (!string.IsNullOrWhiteSpace(dto.GSTNumber))
        {
            var existingCompany = await _context.Companies
                .AnyAsync(c => c.GSTNumber == dto.GSTNumber && c.DeletedAt == null);

            if (existingCompany)
            {
                throw new BusinessRuleException($"A company with GST number '{dto.GSTNumber}' already exists");
            }
        }

        var company = new Company
        {
            CompanyName = dto.CompanyName,
            Address = dto.Address,
            City = dto.City,
            State = dto.State,
            Country = dto.Country,
            GSTNumber = dto.GSTNumber,
            ContactPerson = dto.ContactPerson,
            Designation = dto.Designation,
            Email = dto.Email,
            ContactNumber = dto.ContactNumber,
            DiscountPercentage = dto.DiscountPercentage,
            IsActive = true
        };

        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(company.Id))!;
    }

    public async Task<CompanyDto> UpdateAsync(int id, UpdateCompanyDto dto)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company == null || company.DeletedAt != null)
            throw new NotFoundException(nameof(Company), id);

        // Check if GST number already exists (if provided and different from current)
        if (!string.IsNullOrWhiteSpace(dto.GSTNumber) && dto.GSTNumber != company.GSTNumber)
        {
            var existingCompany = await _context.Companies
                .AnyAsync(c => c.GSTNumber == dto.GSTNumber && c.Id != id && c.DeletedAt == null);

            if (existingCompany)
            {
                throw new BusinessRuleException($"A company with GST number '{dto.GSTNumber}' already exists");
            }
        }

        company.CompanyName = dto.CompanyName;
        company.Address = dto.Address;
        company.City = dto.City;
        company.State = dto.State;
        company.Country = dto.Country;
        company.GSTNumber = dto.GSTNumber;
        company.ContactPerson = dto.ContactPerson;
        company.Designation = dto.Designation;
        company.Email = dto.Email;
        company.ContactNumber = dto.ContactNumber;
        company.DiscountPercentage = dto.DiscountPercentage;

        await _context.SaveChangesAsync();

        return (await GetByIdAsync(id))!;
    }

    public async Task DeleteAsync(int id)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company == null || company.DeletedAt != null)
            throw new NotFoundException(nameof(Company), id);

        company.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<CompanyDto> ActivateAsync(int id)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company == null || company.DeletedAt != null)
            throw new NotFoundException(nameof(Company), id);

        company.IsActive = true;
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(id))!;
    }

    public async Task<CompanyDto> DeactivateAsync(int id)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company == null || company.DeletedAt != null)
            throw new NotFoundException(nameof(Company), id);

        company.IsActive = false;
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(id))!;
    }

    public async Task<List<CompanyDto>> SearchAsync(string searchTerm)
    {
        return await _context.Companies
            .Where(c => c.DeletedAt == null &&
                       (c.CompanyName.Contains(searchTerm) ||
                        (c.City != null && c.City.Contains(searchTerm)) ||
                        (c.ContactPerson != null && c.ContactPerson.Contains(searchTerm))))
            .Select(c => new CompanyDto
            {
                Id = c.Id,
                CompanyName = c.CompanyName,
                Address = c.Address,
                City = c.City,
                State = c.State,
                Country = c.Country,
                GSTNumber = c.GSTNumber,
                ContactPerson = c.ContactPerson,
                Designation = c.Designation,
                Email = c.Email,
                ContactNumber = c.ContactNumber,
                DiscountPercentage = c.DiscountPercentage,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .OrderBy(c => c.CompanyName)
            .ToListAsync();
    }
}
