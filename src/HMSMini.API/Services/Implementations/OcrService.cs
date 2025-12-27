using System.Text.RegularExpressions;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.Common;
using HMSMini.API.Models.DTOs.Guest;
using HMSMini.API.Services.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Tesseract;

namespace HMSMini.API.Services.Implementations;

/// <summary>
/// Service for OCR operations using Tesseract
/// </summary>
public class OcrService : IOcrService
{
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OcrService> _logger;
    private readonly string _tessdataPath;

    public OcrService(
        IWebHostEnvironment environment,
        IConfiguration configuration,
        ILogger<OcrService> logger)
    {
        _environment = environment;
        _configuration = configuration;
        _logger = logger;

        _tessdataPath = _configuration["Ocr:TesseractDataPath"] ?? "wwwroot/tessdata";
    }

    public async Task<OcrResult> ProcessImageAsync(Stream imageStream, string fileName)
    {
        try
        {
            _logger.LogInformation("Processing image for OCR: {FileName}", fileName);

            // Preprocess image for better OCR results
            using var image = await Image.LoadAsync(imageStream);
            using var preprocessedStream = new MemoryStream();

            // Convert to grayscale and enhance contrast
            image.Mutate(x => x
                .Grayscale()
                .Contrast(1.5f));

            await image.SaveAsPngAsync(preprocessedStream);
            preprocessedStream.Position = 0;

            // Perform OCR
            var tessdataPath = Path.Combine(_environment.ContentRootPath, _tessdataPath);

            using var engine = new TesseractEngine(tessdataPath, "eng", EngineMode.Default);
            using var img = Pix.LoadFromMemory(preprocessedStream.ToArray());
            using var page = engine.Process(img);

            var text = page.GetText();
            var confidence = page.GetMeanConfidence();

            _logger.LogInformation("OCR completed with confidence: {Confidence}", confidence);

            return new OcrResult
            {
                ExtractedText = text,
                Confidence = confidence,
                Success = !string.IsNullOrWhiteSpace(text)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing image for OCR");
            throw new Exceptions.ImageProcessingException("Failed to process image for OCR", ex);
        }
    }

    public async Task<GuestInfoDto> ExtractGuestInfoAsync(string extractedText)
    {
        if (string.IsNullOrWhiteSpace(extractedText))
            throw new Exceptions.ImageProcessingException("Extracted text is empty");

        try
        {
            _logger.LogInformation("Extracting guest information from OCR text");

            var guestInfo = new GuestInfoDto();

            // Determine ID type and extract accordingly
            var idType = DetermineIdType(extractedText);

            switch (idType)
            {
                case "Aadhaar":
                    ExtractAadhaarInfo(extractedText, guestInfo);
                    break;
                case "PAN":
                    ExtractPanInfo(extractedText, guestInfo);
                    break;
                case "DrivingLicense":
                    ExtractDrivingLicenseInfo(extractedText, guestInfo);
                    break;
                default:
                    ExtractGenericInfo(extractedText, guestInfo);
                    break;
            }

            _logger.LogInformation("Guest information extracted successfully");

            return await Task.FromResult(guestInfo);
        }
        catch (Exception ex) when (ex is not Exceptions.ImageProcessingException)
        {
            _logger.LogError(ex, "Error extracting guest information");
            throw new Exceptions.ImageProcessingException("Failed to extract guest information", ex);
        }
    }

    public async Task<GuestInfoDto> ProcessImageFileAsync(string imageFilePath)
    {
        if (string.IsNullOrWhiteSpace(imageFilePath) || !File.Exists(imageFilePath))
            throw new NotFoundException("Image file", imageFilePath ?? "null");

        try
        {
            using var fileStream = File.OpenRead(imageFilePath);
            var ocrResult = await ProcessImageAsync(fileStream, Path.GetFileName(imageFilePath));

            if (!ocrResult.Success)
                throw new Exceptions.ImageProcessingException("OCR processing failed - no text extracted");

            return await ExtractGuestInfoAsync(ocrResult.ExtractedText);
        }
        catch (Exception ex) when (ex is not Exceptions.ImageProcessingException && ex is not NotFoundException)
        {
            _logger.LogError(ex, "Error processing image file: {FilePath}", imageFilePath);
            throw new Exceptions.ImageProcessingException("Failed to process image file", ex);
        }
    }

    private string DetermineIdType(string text)
    {
        var upperText = text.ToUpperInvariant();

        if (upperText.Contains("AADHAAR") || upperText.Contains("UIDAI") ||
            Regex.IsMatch(text, @"\d{4}\s*\d{4}\s*\d{4}"))
        {
            return "Aadhaar";
        }

        if (upperText.Contains("INCOME TAX") || upperText.Contains("PERMANENT ACCOUNT NUMBER") ||
            Regex.IsMatch(text, @"[A-Z]{5}\d{4}[A-Z]"))
        {
            return "PAN";
        }

        if (upperText.Contains("DRIVING") || upperText.Contains("LICENCE") || upperText.Contains("LICENSE") ||
            upperText.Contains("TRANSPORT"))
        {
            return "DrivingLicense";
        }

        return "Unknown";
    }

    private void ExtractAadhaarInfo(string text, GuestInfoDto guestInfo)
    {
        // Extract Aadhaar number (12 digits, may have spaces)
        var aadhaarMatch = Regex.Match(text, @"(\d{4}\s*\d{4}\s*\d{4})");
        if (aadhaarMatch.Success)
        {
            guestInfo.IdNumber = aadhaarMatch.Value.Replace(" ", "");
        }

        // Extract name - usually first line or after DOB/Gender
        var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            var cleanLine = line.Trim();
            // Skip lines with Aadhaar number, DOB, Gender keywords
            if (cleanLine.Length > 3 && !cleanLine.Contains("DOB") &&
                !cleanLine.Contains("Male") && !cleanLine.Contains("Female") &&
                !Regex.IsMatch(cleanLine, @"\d{4}") &&
                !cleanLine.Contains("GOVERNMENT") && !cleanLine.Contains("INDIA") &&
                !cleanLine.Contains("AADHAAR") && !cleanLine.Contains("UIDAI"))
            {
                if (string.IsNullOrWhiteSpace(guestInfo.GuestName) && Regex.IsMatch(cleanLine, @"^[A-Za-z\s]+$"))
                {
                    guestInfo.GuestName = cleanLine;
                }
                else if (string.IsNullOrWhiteSpace(guestInfo.Address) && cleanLine.Length > 10)
                {
                    guestInfo.Address = cleanLine;
                }
            }
        }

        // Extract address - usually multi-line after name
        var addressMatch = Regex.Match(text, @"(?:Address|S/O|D/O|C/O)[:\s]+(.*?)(?:\n|$)", RegexOptions.IgnoreCase);
        if (addressMatch.Success && string.IsNullOrWhiteSpace(guestInfo.Address))
        {
            guestInfo.Address = addressMatch.Groups[1].Value.Trim();
        }

        // Extract PIN code and use it to infer city
        var pinMatch = Regex.Match(text, @"\b(\d{6})\b");
        if (pinMatch.Success)
        {
            // Try to extract city name before PIN
            var cityMatch = Regex.Match(text, @"([A-Za-z\s]+)\s*[-,]?\s*" + pinMatch.Value);
            if (cityMatch.Success)
            {
                guestInfo.City = cityMatch.Groups[1].Value.Trim();
            }
        }

        // Extract state - common Indian states
        var statePatterns = new[] { "Maharashtra", "Karnataka", "Tamil Nadu", "Kerala", "Gujarat",
            "Rajasthan", "Punjab", "Haryana", "Delhi", "Uttar Pradesh", "West Bengal", "Telangana", "Andhra Pradesh" };

        foreach (var state in statePatterns)
        {
            if (text.Contains(state, StringComparison.OrdinalIgnoreCase))
            {
                guestInfo.State = state;
                break;
            }
        }

        guestInfo.Country = "India";
    }

    private void ExtractPanInfo(string text, GuestInfoDto guestInfo)
    {
        // Extract PAN number
        var panMatch = Regex.Match(text, @"([A-Z]{5}\d{4}[A-Z])");
        if (panMatch.Success)
        {
            guestInfo.IdNumber = panMatch.Value;
        }

        // Extract name - usually in capital letters
        var nameMatch = Regex.Match(text, @"(?:Name|[Nn]ame)\s*[:\-]?\s*([A-Z\s]+)");
        if (nameMatch.Success)
        {
            guestInfo.GuestName = nameMatch.Groups[1].Value.Trim();
        }
        else
        {
            // Try to find name from lines with capital letters
            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var cleanLine = line.Trim();
                if (cleanLine.Length > 3 && Regex.IsMatch(cleanLine, @"^[A-Z\s]+$") &&
                    !cleanLine.Contains("INCOME") && !cleanLine.Contains("INDIA") &&
                    !cleanLine.Contains("PERMANENT") && !cleanLine.Contains("ACCOUNT"))
                {
                    guestInfo.GuestName = cleanLine;
                    break;
                }
            }
        }

        // Extract father's name
        var fatherMatch = Regex.Match(text, @"(?:Father|Father's Name)\s*[:\-]?\s*([A-Z\s]+)");
        if (fatherMatch.Success)
        {
            guestInfo.Address = "S/O " + fatherMatch.Groups[1].Value.Trim();
        }

        guestInfo.Country = "India";
    }

    private void ExtractDrivingLicenseInfo(string text, GuestInfoDto guestInfo)
    {
        // Extract DL number
        var dlMatch = Regex.Match(text, @"([A-Z]{2}\d{2}\s?\d{11})");
        if (dlMatch.Success)
        {
            guestInfo.IdNumber = dlMatch.Value;
        }

        // Extract name
        var nameMatch = Regex.Match(text, @"(?:Name|[Nn]ame)\s*[:\-]?\s*([A-Za-z\s]+)");
        if (nameMatch.Success)
        {
            guestInfo.GuestName = nameMatch.Groups[1].Value.Trim();
        }

        // Extract address
        var addressMatch = Regex.Match(text, @"(?:Address|Add)\s*[:\-]?\s*(.*?)(?:\n|$)", RegexOptions.IgnoreCase);
        if (addressMatch.Success)
        {
            guestInfo.Address = addressMatch.Groups[1].Value.Trim();
        }

        // Extract PIN code
        var pinMatch = Regex.Match(text, @"\b(\d{6})\b");
        if (pinMatch.Success)
        {
            var cityMatch = Regex.Match(text, @"([A-Za-z\s]+)\s*[-,]?\s*" + pinMatch.Value);
            if (cityMatch.Success)
            {
                guestInfo.City = cityMatch.Groups[1].Value.Trim();
            }
        }

        guestInfo.Country = "India";
    }

    private void ExtractGenericInfo(string text, GuestInfoDto guestInfo)
    {
        _logger.LogWarning("Unable to determine ID type, extracting generic information");

        // Try to extract name (first capitalized line)
        var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            var cleanLine = line.Trim();
            if (cleanLine.Length > 3 && Regex.IsMatch(cleanLine, @"[A-Z][a-z]+"))
            {
                if (string.IsNullOrWhiteSpace(guestInfo.GuestName))
                {
                    guestInfo.GuestName = cleanLine;
                }
                else if (string.IsNullOrWhiteSpace(guestInfo.Address))
                {
                    guestInfo.Address = cleanLine;
                }
            }
        }

        // Try to extract mobile number (10 digits)
        var mobileMatch = Regex.Match(text, @"\b([6-9]\d{9})\b");
        if (mobileMatch.Success)
        {
            guestInfo.MobileNo = mobileMatch.Value;
        }

        // Try to extract PIN code
        var pinMatch = Regex.Match(text, @"\b(\d{6})\b");
        if (pinMatch.Success)
        {
            var cityMatch = Regex.Match(text, @"([A-Za-z\s]+)\s*[-,]?\s*" + pinMatch.Value);
            if (cityMatch.Success)
            {
                guestInfo.City = cityMatch.Groups[1].Value.Trim();
            }
        }
    }
}
