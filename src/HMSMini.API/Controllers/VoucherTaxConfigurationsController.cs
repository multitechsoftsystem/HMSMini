using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Models.DTOs.VoucherTax;
using HMSMini.API.Models.Entities;

namespace HMSMini.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VoucherTaxConfigurationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<VoucherTaxConfigurationsController> _logger;

    public VoucherTaxConfigurationsController(
        ApplicationDbContext context,
        ILogger<VoucherTaxConfigurationsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all voucher tax configurations
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<VoucherTaxConfigDto>>> GetAll([FromQuery] bool activeOnly = false)
    {
        try
        {
            var query = _context.VoucherTaxConfigurations.AsQueryable();

            if (activeOnly)
            {
                query = query.Where(v => v.IsActive);
            }

            var configs = await query
                .OrderBy(v => v.DisplayOrder)
                .ToListAsync();

            var dtos = configs.Select(v => new VoucherTaxConfigDto
            {
                Id = v.Id,
                VoucherType = v.VoucherType,
                CgstPercentage = v.CgstPercentage,
                SgstPercentage = v.SgstPercentage,
                IgstPercentage = v.IgstPercentage,
                IsActive = v.IsActive,
                Description = v.Description,
                SACCode = v.SACCode,
                DisplayOrder = v.DisplayOrder
            }).ToList();

            return Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving voucher tax configurations");
            return StatusCode(500, "An error occurred while retrieving voucher tax configurations");
        }
    }

    /// <summary>
    /// Get a specific voucher tax configuration by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<VoucherTaxConfigDto>> GetById(int id)
    {
        try
        {
            var config = await _context.VoucherTaxConfigurations
                .FirstOrDefaultAsync(v => v.Id == id && v.DeletedAt == null);

            if (config == null)
            {
                return NotFound($"Voucher tax configuration with ID {id} not found");
            }

            return Ok(new VoucherTaxConfigDto
            {
                Id = config.Id,
                VoucherType = config.VoucherType,
                CgstPercentage = config.CgstPercentage,
                SgstPercentage = config.SgstPercentage,
                IgstPercentage = config.IgstPercentage,
                IsActive = config.IsActive,
                Description = config.Description,
                DisplayOrder = config.DisplayOrder
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving voucher tax configuration {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the voucher tax configuration");
        }
    }

    /// <summary>
    /// Create a new voucher tax configuration
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<VoucherTaxConfigDto>> Create([FromBody] CreateVoucherTaxConfigDto dto)
    {
        try
        {
            // Check if voucher type already exists
            var exists = await _context.VoucherTaxConfigurations
                .AnyAsync(v => v.VoucherType == dto.VoucherType && v.DeletedAt == null);

            if (exists)
            {
                return BadRequest($"Voucher tax configuration for type '{dto.VoucherType}' already exists");
            }

            var config = new VoucherTaxConfiguration
            {
                VoucherType = dto.VoucherType,
                CgstPercentage = dto.CgstPercentage,
                SgstPercentage = dto.SgstPercentage,
                IgstPercentage = dto.IgstPercentage,
                IsActive = dto.IsActive,
                Description = dto.Description,
                SACCode = dto.SACCode,
                DisplayOrder = dto.DisplayOrder,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = User.Identity?.Name ?? "System"
            };

            _context.VoucherTaxConfigurations.Add(config);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created voucher tax configuration: {VoucherType}", dto.VoucherType);

            var resultDto = new VoucherTaxConfigDto
            {
                Id = config.Id,
                VoucherType = config.VoucherType,
                CgstPercentage = config.CgstPercentage,
                SgstPercentage = config.SgstPercentage,
                IgstPercentage = config.IgstPercentage,
                IsActive = config.IsActive,
                Description = config.Description,
                DisplayOrder = config.DisplayOrder
            };

            return CreatedAtAction(nameof(GetById), new { id = config.Id }, resultDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating voucher tax configuration");
            return StatusCode(500, "An error occurred while creating the voucher tax configuration");
        }
    }

    /// <summary>
    /// Update an existing voucher tax configuration
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<VoucherTaxConfigDto>> Update(int id, [FromBody] UpdateVoucherTaxConfigDto dto)
    {
        try
        {
            var config = await _context.VoucherTaxConfigurations
                .FirstOrDefaultAsync(v => v.Id == id && v.DeletedAt == null);

            if (config == null)
            {
                return NotFound($"Voucher tax configuration with ID {id} not found");
            }

            // Check if voucher type already exists (excluding current record)
            var exists = await _context.VoucherTaxConfigurations
                .AnyAsync(v => v.VoucherType == dto.VoucherType && v.Id != id && v.DeletedAt == null);

            if (exists)
            {
                return BadRequest($"Voucher tax configuration for type '{dto.VoucherType}' already exists");
            }

            config.VoucherType = dto.VoucherType;
            config.CgstPercentage = dto.CgstPercentage;
            config.SgstPercentage = dto.SgstPercentage;
            config.IgstPercentage = dto.IgstPercentage;
            config.IsActive = dto.IsActive;
            config.Description = dto.Description;
            config.SACCode = dto.SACCode;
            config.DisplayOrder = dto.DisplayOrder;
            config.UpdatedAt = DateTime.UtcNow;
            config.UpdatedBy = User.Identity?.Name ?? "System";

            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated voucher tax configuration: {VoucherType}", dto.VoucherType);

            return Ok(new VoucherTaxConfigDto
            {
                Id = config.Id,
                VoucherType = config.VoucherType,
                CgstPercentage = config.CgstPercentage,
                SgstPercentage = config.SgstPercentage,
                IgstPercentage = config.IgstPercentage,
                IsActive = config.IsActive,
                Description = config.Description,
                DisplayOrder = config.DisplayOrder
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating voucher tax configuration {Id}", id);
            return StatusCode(500, "An error occurred while updating the voucher tax configuration");
        }
    }

    /// <summary>
    /// Delete a voucher tax configuration (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var config = await _context.VoucherTaxConfigurations
                .FirstOrDefaultAsync(v => v.Id == id && v.DeletedAt == null);

            if (config == null)
            {
                return NotFound($"Voucher tax configuration with ID {id} not found");
            }

            // Soft delete
            config.DeletedAt = DateTime.UtcNow;
            config.DeletedBy = User.Identity?.Name ?? "System";

            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted voucher tax configuration: {VoucherType}", config.VoucherType);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting voucher tax configuration {Id}", id);
            return StatusCode(500, "An error occurred while deleting the voucher tax configuration");
        }
    }
}
