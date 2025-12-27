using HMSMini.API.Exceptions;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Services.Implementations;

/// <summary>
/// Service for handling image storage operations
/// </summary>
public class ImageStorageService : IImageStorageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ImageStorageService> _logger;
    private readonly string _uploadPath;
    private readonly long _maxFileSizeInBytes;
    private readonly string[] _allowedExtensions;

    public ImageStorageService(
        IWebHostEnvironment environment,
        IConfiguration configuration,
        ILogger<ImageStorageService> logger)
    {
        _environment = environment;
        _configuration = configuration;
        _logger = logger;

        // Get configuration values
        _uploadPath = _configuration["FileStorage:IdProofPath"] ?? "wwwroot/uploads/idproofs";
        var maxFileSizeInMB = _configuration.GetValue<int>("FileStorage:MaxFileSizeInMB", 10);
        _maxFileSizeInBytes = maxFileSizeInMB * 1024 * 1024;
        _allowedExtensions = _configuration.GetSection("FileStorage:AllowedExtensions").Get<string[]>()
            ?? new[] { ".jpg", ".jpeg", ".png" };
    }

    public async Task<string> SaveImageAsync(IFormFile file, int checkInId, int guestNumber, int photoNumber)
    {
        // Validate file
        if (file == null || file.Length == 0)
            throw new Exceptions.ImageProcessingException("File is empty or null");

        if (file.Length > _maxFileSizeInBytes)
            throw new Exceptions.ImageProcessingException($"File size exceeds maximum allowed size of {_maxFileSizeInBytes / 1024 / 1024}MB");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
            throw new Exceptions.ImageProcessingException($"File type {extension} is not allowed. Allowed types: {string.Join(", ", _allowedExtensions)}");

        try
        {
            // Create directory structure: uploads/idproofs/{checkInId}/
            var checkInFolder = Path.Combine(_uploadPath, checkInId.ToString());
            var fullPath = Path.Combine(_environment.ContentRootPath, checkInFolder);

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            // Generate unique filename: {checkInId}_guest{guestNumber}_photo{photoNumber}_{timestamp}.jpg
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var fileName = $"{checkInId}_guest{guestNumber}_photo{photoNumber}_{timestamp}{extension}";
            var filePath = Path.Combine(fullPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return relative path for database storage
            var relativePath = Path.Combine(checkInFolder, fileName).Replace("\\", "/");

            _logger.LogInformation("Image saved successfully: {FilePath}", relativePath);

            return relativePath;
        }
        catch (Exception ex) when (ex is not Exceptions.ImageProcessingException)
        {
            _logger.LogError(ex, "Error saving image file");
            throw new Exceptions.ImageProcessingException("Failed to save image file", ex);
        }
    }

    public async Task<bool> DeleteImageAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return false;

        try
        {
            var fullPath = GetPhysicalPath(filePath);

            if (File.Exists(fullPath))
            {
                await Task.Run(() => File.Delete(fullPath));
                _logger.LogInformation("Image deleted successfully: {FilePath}", filePath);
                return true;
            }

            _logger.LogWarning("Image file not found for deletion: {FilePath}", filePath);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image file: {FilePath}", filePath);
            return false;
        }
    }

    public async Task<byte[]> GetImageAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new NotFoundException("Image", filePath ?? "null");

        try
        {
            var fullPath = GetPhysicalPath(filePath);

            if (!File.Exists(fullPath))
                throw new NotFoundException("Image", filePath);

            return await File.ReadAllBytesAsync(fullPath);
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            _logger.LogError(ex, "Error reading image file: {FilePath}", filePath);
            throw new Exceptions.ImageProcessingException("Failed to read image file", ex);
        }
    }

    public string GetPhysicalPath(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            throw new ArgumentException("Relative path cannot be null or empty", nameof(relativePath));

        // Normalize path separators
        relativePath = relativePath.Replace("/", Path.DirectorySeparatorChar.ToString());

        return Path.Combine(_environment.ContentRootPath, relativePath);
    }
}
