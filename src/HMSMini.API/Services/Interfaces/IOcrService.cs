using HMSMini.API.Models.DTOs.Common;
using HMSMini.API.Models.DTOs.Guest;

namespace HMSMini.API.Services.Interfaces;

/// <summary>
/// Service for OCR (Optical Character Recognition) operations
/// </summary>
public interface IOcrService
{
    /// <summary>
    /// Processes an image and extracts text using OCR
    /// </summary>
    /// <param name="imageStream">The image stream to process</param>
    /// <param name="fileName">The original file name</param>
    /// <returns>OCR result with extracted text and confidence</returns>
    Task<OcrResult> ProcessImageAsync(Stream imageStream, string fileName);

    /// <summary>
    /// Extracts guest information from OCR text
    /// </summary>
    /// <param name="extractedText">The text extracted from OCR</param>
    /// <returns>Structured guest information</returns>
    Task<GuestInfoDto> ExtractGuestInfoAsync(string extractedText);

    /// <summary>
    /// Processes an image file path and extracts guest information
    /// </summary>
    /// <param name="imageFilePath">The full path to the image file</param>
    /// <returns>Structured guest information</returns>
    Task<GuestInfoDto> ProcessImageFileAsync(string imageFilePath);
}
