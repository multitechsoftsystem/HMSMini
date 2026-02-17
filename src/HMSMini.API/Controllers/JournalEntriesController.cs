using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.JournalEntry;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

[ApiController]
[Route("api/journal-entries")]
[Authorize(Roles = "Admin,Manager")]
public class JournalEntriesController : ControllerBase
{
    private readonly IJournalEntryService _journalEntryService;
    private readonly ILogger<JournalEntriesController> _logger;

    public JournalEntriesController(IJournalEntryService journalEntryService, ILogger<JournalEntriesController> logger)
    {
        _journalEntryService = journalEntryService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(JournalEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JournalEntryDto>> GetById(int id)
    {
        var result = await _journalEntryService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<JournalEntryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<JournalEntryDto>>> GetByDateRange(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] int? financialYearId = null)
    {
        var result = await _journalEntryService.GetByDateRangeAsync(from, to, financialYearId);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(JournalEntryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<JournalEntryDto>> Create([FromBody] CreateJournalEntryDto dto)
    {
        var result = await _journalEntryService.CreateAsync(dto, User.Identity?.Name);
        return Created($"api/journal-entries/{result.Id}", result);
    }

    [HttpPost("{id}/reverse")]
    [ProducesResponseType(typeof(JournalEntryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JournalEntryDto>> Reverse(int id)
    {
        var result = await _journalEntryService.CreateReversalAsync(id, User.Identity?.Name);
        return Created($"api/journal-entries/{result.Id}", result);
    }
}
