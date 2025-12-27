namespace HMSMini.API.Services.Interfaces;

/// <summary>
/// Service for handling image storage operations
/// </summary>
public interface IImageStorageService
{
    /// <summary>
    /// Saves an uploaded image file to storage
    /// </summary>
    /// <param name="file">The image file to save</param>
    /// <param name="checkInId">The check-in ID</param>
    /// <param name="guestNumber">The guest number (1-3)</param>
    /// <param name="photoNumber">The photo number (1 or 2)</param>
    /// <returns>The relative file path where the image was saved</returns>
    Task<string> SaveImageAsync(IFormFile file, int checkInId, int guestNumber, int photoNumber);

    /// <summary>
    /// Deletes an image file from storage
    /// </summary>
    /// <param name="filePath">The relative file path to delete</param>
    /// <returns>True if deleted successfully, false otherwise</returns>
    Task<bool> DeleteImageAsync(string filePath);

    /// <summary>
    /// Retrieves an image file from storage
    /// </summary>
    /// <param name="filePath">The relative file path to retrieve</param>
    /// <returns>The image file bytes</returns>
    Task<byte[]> GetImageAsync(string filePath);

    /// <summary>
    /// Gets the full physical path from a relative path
    /// </summary>
    /// <param name="relativePath">The relative file path</param>
    /// <returns>The full physical path</returns>
    string GetPhysicalPath(string relativePath);
}
