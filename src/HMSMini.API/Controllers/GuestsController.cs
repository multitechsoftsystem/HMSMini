using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.Guest;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

/// <summary>
/// Manages guest information
/// </summary>
[ApiController]
[Route("api/[controller]")]
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
    /// Update guest information
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(GuestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GuestDto>> Update(int id, [FromBody] CreateGuestDto dto)
    {
        var guest = await _guestService.UpdateAsync(id, dto);
        return Ok(guest);
    }

    /// <summary>
    /// Delete a guest
    /// </summary>
    [HttpDelete("{id}")]
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
    [ProducesResponseType(typeof(GuestInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GuestInfoDto>> ProcessOcr(int id, [FromQuery] int photoNumber = 1)
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

        // Process OCR
        var guestInfo = await _ocrService.ProcessImageFileAsync(fullPath);

        _logger.LogInformation("OCR processed for guest {GuestId}, photo {PhotoNumber}", id, photoNumber);

        return Ok(guestInfo);
    }

    /// <summary>
    /// Retrieve uploaded photo for a guest
    /// </summary>
    /// <param name="id">Guest ID</param>
    /// <param name="photoNumber">Photo number (1 or 2)</param>
    [HttpGet("{id}/photos/{photoNumber}")]
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
}
