using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.Guest;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

/// <summary>
/// Manages guest information
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GuestsController : ControllerBase
{
    private readonly IGuestService _guestService;
    private readonly IImageStorageService _imageStorageService;
    private readonly IOcrService _ocrService;
    private readonly ILogger<GuestsController> _logger;

    public GuestsController(
        IGuestService guestService,
        IImageStorageService imageStorageService,
        IOcrService ocrService,
        ILogger<GuestsController> logger)
    {
        _guestService = guestService;
        _imageStorageService = imageStorageService;
        _ocrService = ocrService;
        _logger = logger;
    }

    /// <summary>
    /// Get guest by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GuestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GuestDto>> GetById(int id)
    {
        var guest = await _guestService.GetByIdAsync(id);
        return Ok(guest);
    }

    /// <summary>
    /// Get all guests for a check-in
    /// </summary>
    [HttpGet("checkin/{checkInId}")]
    [ProducesResponseType(typeof(List<GuestDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GuestDto>>> GetByCheckInId(int checkInId)
    {
        var guests = await _guestService.GetByCheckInIdAsync(checkInId);
        return Ok(guests);
    }

    /// <summary>
    /// Add a new guest to an existing check-in
    /// </summary>
    [HttpPost("checkin/{checkInId}")]
    [Authorize(Roles = "Admin,Manager,Receptionist")]
    [ProducesResponseType(typeof(GuestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GuestDto>> AddGuestToCheckIn(int checkInId, [FromBody] CreateGuestDto dto)
    {
        var guest = await _guestService.CreateAsync(checkInId, dto);
        return CreatedAtAction(nameof(GetById), new { id = guest.Id }, guest);
    }

    /// <summary>
    /// Update guest information
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager,Receptionist")]
    [ProducesResponseType(typeof(GuestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GuestDto>> Update(int id, [FromBody] CreateGuestDto dto)
    {
        var guest = await _guestService.UpdateAsync(id, dto);
        return Ok(guest);
    }

    /// <summary>
    /// Check out an individual guest
    /// </summary>
    [HttpPost("{id}/checkout")]
    [Authorize(Roles = "Admin,Manager,Receptionist")]
    [ProducesResponseType(typeof(GuestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GuestDto>> CheckOutGuest(int id)
    {
        var guest = await _guestService.CheckOutGuestAsync(id);
        return Ok(guest);
    }

    /// <summary>
    /// Delete a guest
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _guestService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Upload ID proof photo for a guest
    /// </summary>
    /// <param name="id">Guest ID</param>
    /// <param name="photoNumber">Photo number (1 or 2)</param>
    /// <param name="file">The image file</param>
    [HttpPost("{id}/upload-photo")]
    [Consumes("multipart/form-data")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Roles = "Admin,Manager,Receptionist")]
    [ProducesResponseType(typeof(GuestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GuestDto>> UploadPhoto(int id, [FromForm] int photoNumber, [FromForm] IFormFile file)
    {
        // Validate photo number
        if (photoNumber < 1 || photoNumber > 2)
            return BadRequest("Photo number must be 1 or 2");

        // Get guest to retrieve check-in ID and guest number
        var guest = await _guestService.GetByIdAsync(id);

        // Save the image
        var filePath = await _imageStorageService.SaveImageAsync(file, guest.CheckInId, guest.GuestNumber, photoNumber);

        // Update guest record with photo path
        var updatedGuest = await _guestService.UpdatePhotoPathAsync(id, photoNumber, filePath);

        _logger.LogInformation("Photo {PhotoNumber} uploaded for guest {GuestId}: {FilePath}", photoNumber, id, filePath);

        return Ok(updatedGuest);
    }

    /// <summary>
    /// Process OCR on uploaded ID proof photo
    /// </summary>
    /// <param name="id">Guest ID</param>
    /// <param name="photoNumber">Photo number to process (1 or 2)</param>
    [HttpPost("{id}/process-ocr")]
    [Authorize(Roles = "Admin,Manager,Receptionist")]
    [ProducesResponseType(typeof(OcrReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OcrReviewDto>> ProcessOcr(int id, [FromQuery] int photoNumber = 1)
    {
        // Validate photo number
        if (photoNumber < 1 || photoNumber > 2)
            return BadRequest("Photo number must be 1 or 2");

        // Get guest
        var guest = await _guestService.GetByIdAsync(id);

        // Get the photo path
        var photoPath = photoNumber == 1 ? guest.Photo1Path : guest.Photo2Path;

        if (string.IsNullOrWhiteSpace(photoPath))
            return BadRequest($"Photo {photoNumber} has not been uploaded for this guest");

        // Get full physical path
        var fullPath = _imageStorageService.GetPhysicalPath(photoPath);

        // Process OCR - get raw text first
        using var fileStream = System.IO.File.OpenRead(fullPath);
        var ocrResult = await _ocrService.ProcessImageAsync(fileStream, Path.GetFileName(fullPath));

        // Extract guest info from the raw text
        var guestInfo = await _ocrService.ExtractGuestInfoAsync(ocrResult.ExtractedText);

        // Create review DTO
        var reviewDto = new OcrReviewDto
        {
            RawText = ocrResult.ExtractedText,
            Confidence = ocrResult.Confidence,
            GuestInfo = guestInfo
        };

        _logger.LogInformation("OCR processed for guest {GuestId}, photo {PhotoNumber}", id, photoNumber);

        return Ok(reviewDto);
    }

    /// <summary>
    /// Retrieve uploaded photo for a guest
    /// </summary>
    /// <param name="id">Guest ID</param>
    /// <param name="photoNumber">Photo number (1 or 2)</param>
    [HttpGet("{id}/photos/{photoNumber}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPhoto(int id, int photoNumber)
    {
        // Validate photo number
        if (photoNumber < 1 || photoNumber > 2)
            return BadRequest("Photo number must be 1 or 2");

        // Get guest
        var guest = await _guestService.GetByIdAsync(id);

        // Get the photo path
        var photoPath = photoNumber == 1 ? guest.Photo1Path : guest.Photo2Path;

        if (string.IsNullOrWhiteSpace(photoPath))
            return NotFound($"Photo {photoNumber} not found for guest {id}");

        // Get image bytes
        var imageBytes = await _imageStorageService.GetImageAsync(photoPath);

        // Determine content type from file extension
        var extension = Path.GetExtension(photoPath).ToLowerInvariant();
        var contentType = extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream"
        };

        return File(imageBytes, contentType);
    }

    /// <summary>
    /// Search guests by mobile number
    /// </summary>
    [HttpGet("search/mobile")]
    [ProducesResponseType(typeof(List<GuestDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GuestDto>>> SearchByMobile([FromQuery] string mobile)
    {
        if (string.IsNullOrWhiteSpace(mobile))
            return Ok(new List<GuestDto>());

        var guests = await _guestService.SearchGuestsByMobileAsync(mobile);
        return Ok(guests);
    }

    /// <summary>
    /// Search guests by name
    /// </summary>
    [HttpGet("search/name")]
    [ProducesResponseType(typeof(List<GuestDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GuestDto>>> SearchByName([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Ok(new List<GuestDto>());

        var guests = await _guestService.SearchGuestsByNameAsync(name);
        return Ok(guests);
    }

    /// <summary>
    /// Search guests by mobile or name
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<GuestDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GuestDto>>> Search([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Ok(new List<GuestDto>());

        var guests = await _guestService.SearchGuestsAsync(query);
        return Ok(guests);
    }

    /// <summary>
    /// Process OCR directly from uploaded image (no guest creation required)
    /// </summary>
    [HttpPost("ocr/direct")]
    [Consumes("multipart/form-data")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Roles = "Admin,Manager,Receptionist")]
    [ProducesResponseType(typeof(OcrReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OcrReviewDto>> ProcessOcrDirect([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        if (file.Length > 10 * 1024 * 1024) // 10MB limit
            return BadRequest("File size must be less than 10MB");

        using var stream = file.OpenReadStream();
        var ocrResult = await _ocrService.ProcessImageAsync(stream, file.FileName);

        // Extract guest info from the raw text
        var guestInfo = await _ocrService.ExtractGuestInfoAsync(ocrResult.ExtractedText);

        // Create review DTO
        var reviewDto = new OcrReviewDto
        {
            RawText = ocrResult.ExtractedText,
            Confidence = ocrResult.Confidence,
            GuestInfo = guestInfo
        };

        _logger.LogInformation("Direct OCR processed for file {FileName}", file.FileName);

        return Ok(reviewDto);
    }
}
