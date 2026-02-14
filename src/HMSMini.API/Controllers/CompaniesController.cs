using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.Company;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

/// <summary>
/// Controller for managing companies
/// </summary>
[ApiController]
[Route("api/companies")]
[Authorize]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyService _companyService;
    private readonly ILogger<CompaniesController> _logger;

    public CompaniesController(
        ICompanyService companyService,
        ILogger<CompaniesController> logger)
    {
        _companyService = companyService;
        _logger = logger;
    }

    /// <summary>
    /// Get all companies
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<CompanyListDto>>> GetAll([FromQuery] bool includeInactive = false)
    {
        var companies = await _companyService.GetAllAsync(includeInactive);
        return Ok(companies);
    }

    /// <summary>
    /// Get active companies (for dropdown)
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<List<CompanyDto>>> GetActiveCompanies()
    {
        var companies = await _companyService.GetActiveCompaniesAsync();
        return Ok(companies);
    }

    /// <summary>
    /// Get company by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CompanyDto>> GetById(int id)
    {
        var company = await _companyService.GetByIdAsync(id);
        if (company == null)
        {
            return NotFound($"Company with ID {id} not found.");
        }
        return Ok(company);
    }

    /// <summary>
    /// Search companies by name, city, or contact person
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<List<CompanyDto>>> Search([FromQuery] string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return BadRequest("Search term cannot be empty.");
        }

        var companies = await _companyService.SearchAsync(term);
        return Ok(companies);
    }

    /// <summary>
    /// Create a new company
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<CompanyDto>> Create([FromBody] CreateCompanyDto dto)
    {
        var company = await _companyService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = company.Id }, company);
    }

    /// <summary>
    /// Update a company
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<CompanyDto>> Update(int id, [FromBody] UpdateCompanyDto dto)
    {
        var company = await _companyService.UpdateAsync(id, dto);
        return Ok(company);
    }

    /// <summary>
    /// Delete a company (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        await _companyService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Activate a company
    /// </summary>
    [HttpPost("{id}/activate")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<CompanyDto>> Activate(int id)
    {
        var company = await _companyService.ActivateAsync(id);
        return Ok(company);
    }

    /// <summary>
    /// Deactivate a company
    /// </summary>
    [HttpPost("{id}/deactivate")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<CompanyDto>> Deactivate(int id)
    {
        var company = await _companyService.DeactivateAsync(id);
        return Ok(company);
    }
}
