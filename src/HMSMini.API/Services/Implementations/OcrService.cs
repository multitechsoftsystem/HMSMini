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

            var rawText = page.GetText();
            var confidence = page.GetMeanConfidence();

            // Filter out non-English/regional language characters
            var cleanedText = CleanTextToEnglishOnly(rawText);

            _logger.LogInformation("OCR completed with confidence: {Confidence}", confidence);
            _logger.LogInformation("Cleaned OCR Text:\n{CleanedText}", cleanedText);

            return new OcrResult
            {
                ExtractedText = cleanedText,
                Confidence = confidence,
                Success = !string.IsNullOrWhiteSpace(cleanedText)
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
        // Check if Aadhar is masked
        bool isMasked = text.Contains("XXXX");

        if (!isMasked)
        {
            // Try to extract unmasked Aadhaar number (12 digits, may have spaces)
            // Look for 12-digit pattern on a single line, with specific Aadhar context
            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                // Pattern 1: Line with exactly 12 digits (with spaces): "8996 9810 3068"
                var aadhaarMatch = Regex.Match(line, @"^\s*(\d{4}\s+\d{4}\s+\d{4})\s*$");
                if (aadhaarMatch.Success)
                {
                    guestInfo.IdNumber = aadhaarMatch.Groups[1].Value.Replace(" ", "");
                    break;
                }

                // Pattern 2: Line with "o" or "0" prefix and 12 digits: "o 8996 9810 3068"
                var aadhaarWithPrefixMatch = Regex.Match(line, @"^[o0]\s+(\d{4}\s+\d{4}\s+\d{4})\s*$", RegexOptions.IgnoreCase);
                if (aadhaarWithPrefixMatch.Success)
                {
                    guestInfo.IdNumber = aadhaarWithPrefixMatch.Groups[1].Value.Replace(" ", "");
                    break;
                }
            }
        }

        // If masked or no Aadhar found, try to extract VID (16 digits)
        if (string.IsNullOrWhiteSpace(guestInfo.IdNumber))
        {
            // Pattern: VID : 9198 9929 34347024 or VID: 1234567890123456
            var vidMatch = Regex.Match(text, @"VID[\s:]+(\d{4}\s*\d{4}\s*\d{4}\s*\d{4})", RegexOptions.IgnoreCase);
            if (vidMatch.Success)
            {
                guestInfo.IdNumber = vidMatch.Groups[1].Value.Replace(" ", "");
            }
        }

        // Process lines to extract information
        var allLines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var addressLines = new List<string>();

        for (int i = 0; i < allLines.Length; i++)
        {
            var line = allLines[i].Trim();

            // Extract Mobile (pattern: Mobile: 9422005779 or just 10 digits alone)
            var mobileMatch = Regex.Match(line, @"Mobile[\s:]+(\d{10})", RegexOptions.IgnoreCase);
            if (mobileMatch.Success)
            {
                guestInfo.MobileNo = mobileMatch.Groups[1].Value;
            }
            // Also check for standalone 10-digit numbers starting with 6-9
            else if (string.IsNullOrWhiteSpace(guestInfo.MobileNo))
            {
                var standaloneMobileMatch = Regex.Match(line, @"^([6-9]\d{9})$");
                if (standaloneMobileMatch.Success)
                {
                    guestInfo.MobileNo = standaloneMobileMatch.Groups[1].Value;
                }
            }

            // Extract Date of Birth (patterns: DOB: DD/MM/YYYY, Date of Birth: DD/MM/YYYY, DOB DD/MM/YYYY)
            if (guestInfo.DateOfBirth == null)
            {
                var dobMatch = Regex.Match(line, @"(?:DOB|Date of Birth|YOB|Year of Birth)[\s:]+(\d{2})[\/\-\.](\d{2})[\/\-\.](\d{4})", RegexOptions.IgnoreCase);
                if (dobMatch.Success)
                {
                    try
                    {
                        var day = int.Parse(dobMatch.Groups[1].Value);
                        var month = int.Parse(dobMatch.Groups[2].Value);
                        var year = int.Parse(dobMatch.Groups[3].Value);
                        guestInfo.DateOfBirth = new DateTime(year, month, day);
                    }
                    catch
                    {
                        // Invalid date format, skip
                    }
                }
            }

            // Extract State before PIN Code (patterns: "Goa 403804", "Gujarat, 382350", "DIST:Ahmedabad, Gujarat, 382350")
            if (string.IsNullOrWhiteSpace(guestInfo.State))
            {
                // Pattern: State name followed by PIN code
                var stateWithPinMatch = Regex.Match(line, @"([A-Za-z\s]+)\s*,?\s*(\d{6})", RegexOptions.IgnoreCase);
                if (stateWithPinMatch.Success)
                {
                    var potentialState = stateWithPinMatch.Groups[1].Value.Trim();
                    // Check if it's a known state
                    var knownStates = new[] { "Maharashtra", "Karnataka", "Tamil Nadu", "Kerala", "Gujarat",
                        "Rajasthan", "Punjab", "Haryana", "Delhi", "Uttar Pradesh", "West Bengal", "Telangana",
                        "Andhra Pradesh", "Madhya Pradesh", "Bihar", "Odisha", "Assam", "Jharkhand", "Chhattisgarh",
                        "Uttarakhand", "Goa", "Himachal Pradesh", "Tripura", "Meghalaya", "Manipur", "Nagaland", "Mizoram", "Sikkim" };

                    foreach (var state in knownStates)
                    {
                        if (potentialState.Contains(state, StringComparison.OrdinalIgnoreCase))
                        {
                            guestInfo.State = state;
                            break;
                        }
                    }
                }
            }

            // Extract State (pattern: St Maharashtra, or State: Maharashtra)
            if (string.IsNullOrWhiteSpace(guestInfo.State))
            {
                var stateMatch = Regex.Match(line, @"(?:St|State)[\s:]+([A-Za-z\s]+?)[\s,]*$", RegexOptions.IgnoreCase);
                if (stateMatch.Success)
                {
                    var stateName = stateMatch.Groups[1].Value.Trim();
                    stateName = stateName.TrimEnd(',', '.', ';', ':');
                    stateName = FixStateNameOcrErrors(stateName);
                    if (!string.IsNullOrWhiteSpace(stateName))
                    {
                        guestInfo.State = stateName;
                    }
                }
            }

            // Extract District/City (pattern: Disrct: Pune, DIST:Ahmedabad)
            if (string.IsNullOrWhiteSpace(guestInfo.City))
            {
                var districtMatch = Regex.Match(line, @"(?:Disrct|District|DIST)[\s:]+([A-Za-z\s]+)", RegexOptions.IgnoreCase);
                if (districtMatch.Success)
                {
                    guestInfo.City = districtMatch.Groups[1].Value.Trim().Split(',')[0].Trim();
                }
            }

            // Extract PO (Post Office)
            if (string.IsNullOrWhiteSpace(guestInfo.City))
            {
                var poMatch = Regex.Match(line, @"PO[\s:]+([A-Za-z\s]+)", RegexOptions.IgnoreCase);
                if (poMatch.Success)
                {
                    guestInfo.City = poMatch.Groups[1].Value.Trim().Split(',')[0].TrimEnd(',', '.');
                }
            }

            // Extract VIC (vicinity/area) info
            if (string.IsNullOrWhiteSpace(guestInfo.City))
            {
                var vicMatch = Regex.Match(line, @"VIC[\s:]+([A-Za-z\s]+)", RegexOptions.IgnoreCase);
                if (vicMatch.Success)
                {
                    guestInfo.City = vicMatch.Groups[1].Value.Trim().TrimEnd(',', '.');
                }
            }
        }

        // Extract Name - look for capitalized name pattern after enrollment number or near top
        for (int i = 0; i < allLines.Length; i++)
        {
            var line = allLines[i].Trim();

            // Look for name pattern - Try multiple patterns
            // Pattern 1: Three capitalized words (First Middle Last)
            if (string.IsNullOrWhiteSpace(guestInfo.GuestName) &&
                Regex.IsMatch(line, @"^[A-Z][a-z]+\s+[A-Z][a-z]+\s+[A-Z][a-z]+") &&
                !line.Contains("Government") &&
                !line.Contains("India") &&
                line.Length > 5 && line.Length < 50)
            {
                var cleanName = Regex.Replace(line, @"[:\(\)]", "").Trim();
                if (Regex.IsMatch(cleanName, @"^[A-Za-z\s]+$"))
                {
                    guestInfo.GuestName = cleanName;
                }
            }

            // Pattern 2: Two capitalized words (First Last)
            if (string.IsNullOrWhiteSpace(guestInfo.GuestName) &&
                Regex.IsMatch(line, @"^[A-Z][a-z]+\s+[A-Z][a-z]+$") &&
                !line.Contains("Government") &&
                !line.Contains("India") &&
                !line.Contains("Enrolment") &&
                line.Length > 5 && line.Length < 40)
            {
                var cleanName = Regex.Replace(line, @"[:\(\)]", "").Trim();
                if (Regex.IsMatch(cleanName, @"^[A-Za-z\s]+$"))
                {
                    guestInfo.GuestName = cleanName;
                }
            }
        }

        // Extract Address - First check for "Address:" label
        bool addressFound = false;
        int addressStartIndex = -1;

        for (int i = 0; i < allLines.Length; i++)
        {
            var line = allLines[i].Trim();

            // Look for "Address:" label
            if (Regex.IsMatch(line, @"^Address[\s:]+", RegexOptions.IgnoreCase))
            {
                // Extract everything after "Address:"
                var addressText = Regex.Replace(line, @"^Address[\s:]+", "", RegexOptions.IgnoreCase).Trim();
                if (!string.IsNullOrWhiteSpace(addressText))
                {
                    addressLines.Add(addressText);
                    addressStartIndex = i;
                    addressFound = true;
                }
            }
            // Continue collecting address lines after "Address:" label
            else if (addressFound && addressLines.Count < 3)
            {
                // Stop at structured fields
                if (Regex.IsMatch(line, @"(VIC|PO|Disrct|District|DIST|St |State|PIN|Mobile|DOB|Aadhaar|XXXX)", RegexOptions.IgnoreCase))
                {
                    break;
                }
                // Collect address continuation lines
                if (line.Length > 5 && Regex.IsMatch(line, @"[A-Za-z]") && !Regex.IsMatch(line, @"^\d{10}$"))
                {
                    addressLines.Add(line.TrimEnd(','));
                }
            }
        }

        // If no "Address:" label found, collect lines after name
        if (!addressFound)
        {
            int nameLineIndex = -1;

            // Find where name appears
            for (int i = 0; i < allLines.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(guestInfo.GuestName) && allLines[i].Contains(guestInfo.GuestName.Split(' ')[0]))
                {
                    nameLineIndex = i;
                    break;
                }
            }

            // Collect address lines after name
            if (nameLineIndex >= 0)
            {
                for (int i = nameLineIndex + 1; i < allLines.Length; i++)
                {
                    var line = allLines[i].Trim();

                    // Stop at structured fields
                    if (Regex.IsMatch(line, @"(VIC|PO|Disrct|District|DIST|St |State|PIN|Mobile|DOB|Aadhaar|XXXX)", RegexOptions.IgnoreCase))
                    {
                        break;
                    }

                    // Collect address lines
                    if (line.Length > 5 && Regex.IsMatch(line, @"[A-Za-z]") && !Regex.IsMatch(line, @"^\d{10}$"))
                    {
                        addressLines.Add(line.TrimEnd(','));
                    }
                }
            }
        }

        // Combine address lines
        if (addressLines.Any())
        {
            guestInfo.Address = string.Join(", ", addressLines.Take(3));
        }

        guestInfo.Country = "India";

        // Log extracted information for debugging
        _logger.LogInformation("Aadhar OCR Extraction Results:");
        _logger.LogInformation("  Name: {Name}", guestInfo.GuestName ?? "(empty)");
        _logger.LogInformation("  Address: {Address}", guestInfo.Address ?? "(empty)");
        _logger.LogInformation("  City: {City}", guestInfo.City ?? "(empty)");
        _logger.LogInformation("  State: {State}", guestInfo.State ?? "(empty)");
        _logger.LogInformation("  Mobile: {Mobile}", guestInfo.MobileNo ?? "(empty)");
        _logger.LogInformation("  ID Number: {IdNumber}", guestInfo.IdNumber ?? "(empty)");
    }

    private string FixStateNameOcrErrors(string stateName)
    {
        // Handle common OCR errors in state names
        var stateMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Maharashia", "Maharashtra" },
            { "Karnatka", "Karnataka" },
            { "Tamilnadu", "Tamil Nadu" },
            { "Uttarpradesh", "Uttar Pradesh" },
            { "Westbengal", "West Bengal" },
            { "Andhrapradesh", "Andhra Pradesh" },
            { "Madhyapradesh", "Madhya Pradesh" }
        };

        foreach (var kvp in stateMap)
        {
            if (stateName.Equals(kvp.Key, StringComparison.OrdinalIgnoreCase))
            {
                return kvp.Value;
            }
        }

        return stateName;
    }

    private string ExpandStateAbbreviation(string stateCode)
    {
        // Expand 2-letter state codes to full names
        var stateCodeMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "MH", "Maharashtra" },
            { "KA", "Karnataka" },
            { "TN", "Tamil Nadu" },
            { "KL", "Kerala" },
            { "GJ", "Gujarat" },
            { "RJ", "Rajasthan" },
            { "PB", "Punjab" },
            { "HR", "Haryana" },
            { "DL", "Delhi" },
            { "UP", "Uttar Pradesh" },
            { "WB", "West Bengal" },
            { "TG", "Telangana" },
            { "TS", "Telangana" },
            { "AP", "Andhra Pradesh" },
            { "MP", "Madhya Pradesh" },
            { "BR", "Bihar" },
            { "OR", "Odisha" },
            { "AS", "Assam" },
            { "JH", "Jharkhand" },
            { "CG", "Chhattisgarh" },
            { "UK", "Uttarakhand" },
            { "GA", "Goa" },
            { "HP", "Himachal Pradesh" }
        };

        if (stateCodeMap.TryGetValue(stateCode, out var fullStateName))
        {
            return fullStateName;
        }

        return stateCode;
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

        // Extract Date of Birth (patterns: DOB: DD/MM/YYYY, Date of Birth: DD/MM/YYYY)
        var dobMatch = Regex.Match(text, @"(?:DOB|Date of Birth)[\s:]+(\d{2})[\/\-](\d{2})[\/\-](\d{4})", RegexOptions.IgnoreCase);
        if (dobMatch.Success)
        {
            try
            {
                var day = int.Parse(dobMatch.Groups[1].Value);
                var month = int.Parse(dobMatch.Groups[2].Value);
                var year = int.Parse(dobMatch.Groups[3].Value);
                guestInfo.DateOfBirth = new DateTime(year, month, day);
            }
            catch
            {
                // Invalid date format, skip
            }
        }

        guestInfo.Country = "India";
    }

    private void ExtractDrivingLicenseInfo(string text, GuestInfoDto guestInfo)
    {
        // Extract DL number - try multiple patterns
        // Pattern 1: DL No: label with noise (e.g., "DL No:@ MH12 200202845 S")
        var dlLabelMatch = Regex.Match(text, @"DL\s*No[\s:@\.]+([A-Z]{2}\d{2}\s?\d{6,11})", RegexOptions.IgnoreCase);
        if (dlLabelMatch.Success)
        {
            guestInfo.IdNumber = dlLabelMatch.Groups[1].Value.Replace(" ", "");
        }
        else
        {
            // Pattern 2: Standard format MH14 20160034761 or MH1420160034761 (2 letters, 13 digits total)
            var dlMatch = Regex.Match(text, @"([A-Z]{2}\d{2}\s?\d{11})");
            if (dlMatch.Success)
            {
                guestInfo.IdNumber = dlMatch.Groups[1].Value.Replace(" ", "");
            }
            else
            {
                // Pattern 3: More flexible - 2 letters followed by digits (with optional spaces/hyphens)
                dlMatch = Regex.Match(text, @"([A-Z]{2}[\s\-]?\d{2}[\s\-]?\d{6,11})");
                if (dlMatch.Success)
                {
                    guestInfo.IdNumber = Regex.Replace(dlMatch.Groups[1].Value, @"[\s\-]", "");
                }
            }
        }

        // Extract name
        var nameMatch = Regex.Match(text, @"(?:Name|[Nn]ame)\s*[:\-]?\s*([A-Za-z\s]+)");
        if (nameMatch.Success)
        {
            guestInfo.GuestName = nameMatch.Groups[1].Value.Trim();
        }

        // Extract address - collect multiple address lines
        var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var addressLines = new List<string>();
        bool collectingAddress = false;

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();

            // Start collecting after "Address:" label
            if (Regex.IsMatch(line, @"(?:Address|Add)[\s:]+", RegexOptions.IgnoreCase))
            {
                var addressText = Regex.Replace(line, @"(?:Address|Add)[\s:]+", "", RegexOptions.IgnoreCase).Trim();
                if (!string.IsNullOrWhiteSpace(addressText))
                {
                    addressLines.Add(addressText);
                }
                collectingAddress = true;
            }
            // Continue collecting address lines
            else if (collectingAddress && addressLines.Count < 3)
            {
                // Stop at PIN code, DOB, or DL number
                if (Regex.IsMatch(line, @"^\d{6}$") ||
                    Regex.IsMatch(line, @"DOB|Date of Birth|DL|Licence|License", RegexOptions.IgnoreCase))
                {
                    break;
                }

                if (line.Length > 5 && Regex.IsMatch(line, @"[A-Za-z]"))
                {
                    addressLines.Add(line);
                }
            }
        }

        if (addressLines.Any())
        {
            guestInfo.Address = string.Join(", ", addressLines);
        }

        // Extract City from lines containing city name before PIN code
        // Look for pattern like "City: Mumbai" or "Dist: Pune" or city name with PIN
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();

            // Pattern 1: City label (City:, District:, Dist:, DIST:)
            if (string.IsNullOrWhiteSpace(guestInfo.City))
            {
                var cityMatch = Regex.Match(line, @"(?:City|District|Dist|DIST)[\s:]+([A-Za-z\s]+)", RegexOptions.IgnoreCase);
                if (cityMatch.Success)
                {
                    var cityName = cityMatch.Groups[1].Value.Trim().Split(',')[0].Split(new[] { '\t', '-' })[0].Trim().TrimEnd(',', '.', ';');
                    // Avoid extracting PIN codes as city
                    if (!Regex.IsMatch(cityName, @"^\d+$") && cityName.Length >= 3)
                    {
                        guestInfo.City = cityName;
                    }
                }
            }

            // Pattern 2: City,State abbreviation (e.g., "PUNE,MH" or "PUNE v/ TY,PUNE,MH")
            if (string.IsNullOrWhiteSpace(guestInfo.City))
            {
                var cityStateMatch = Regex.Match(line, @"([A-Z][A-Za-z]+),\s*([A-Z]{2})\b", RegexOptions.IgnoreCase);
                if (cityStateMatch.Success)
                {
                    var potentialCity = cityStateMatch.Groups[1].Value.Trim();
                    if (potentialCity.Length >= 3 && !Regex.IsMatch(potentialCity, @"^\d+$"))
                    {
                        guestInfo.City = potentialCity;

                        // Extract state abbreviation
                        if (string.IsNullOrWhiteSpace(guestInfo.State))
                        {
                            var stateAbbr = cityStateMatch.Groups[2].Value.Trim();
                            // Try to expand state abbreviation if needed
                            guestInfo.State = stateAbbr;
                        }
                    }
                }
            }

            // Pattern 3: City name followed by state and PIN (e.g., "Mumbai Maharashtra 400001")
            if (string.IsNullOrWhiteSpace(guestInfo.City))
            {
                var cityStatePinMatch = Regex.Match(line, @"([A-Za-z\s]+?)\s+([A-Za-z\s]+?)\s+(\d{6})");
                if (cityStatePinMatch.Success)
                {
                    var potentialCity = cityStatePinMatch.Groups[1].Value.Trim();
                    // Ensure it's not just a random word and not a state name
                    if (potentialCity.Length >= 4 && !Regex.IsMatch(potentialCity, @"^\d+$"))
                    {
                        guestInfo.City = potentialCity;

                        // Also extract state
                        var potentialState = cityStatePinMatch.Groups[2].Value.Trim();
                        if (string.IsNullOrWhiteSpace(guestInfo.State))
                        {
                            guestInfo.State = FixStateNameOcrErrors(potentialState);
                        }
                    }
                }
            }

            // Pattern 4: Look for common city/address patterns before PIN code line
            if (string.IsNullOrWhiteSpace(guestInfo.City) && i + 1 < lines.Length)
            {
                var nextLine = lines[i + 1].Trim();
                // If next line is a PIN code, current line might contain city
                if (Regex.IsMatch(nextLine, @"PIN\s*\d{6}", RegexOptions.IgnoreCase) || Regex.IsMatch(nextLine, @"^\d{6}$"))
                {
                    // Extract last capitalized word from current line as potential city
                    var cityInLineMatch = Regex.Match(line, @"([A-Z][A-Za-z]+)(?:\s*,|\s+[A-Z]{2}\b|$)");
                    if (cityInLineMatch.Success)
                    {
                        var potentialCity = cityInLineMatch.Groups[1].Value.Trim();
                        if (potentialCity.Length >= 3 && !potentialCity.Equals("Add", StringComparison.OrdinalIgnoreCase))
                        {
                            guestInfo.City = potentialCity;
                        }
                    }
                }
            }
        }

        // Extract State if not already found
        if (string.IsNullOrWhiteSpace(guestInfo.State))
        {
            var knownStates = new[] { "Maharashtra", "Karnataka", "Tamil Nadu", "Kerala", "Gujarat",
                "Rajasthan", "Punjab", "Haryana", "Delhi", "Uttar Pradesh", "West Bengal", "Telangana",
                "Andhra Pradesh", "Madhya Pradesh", "Bihar", "Odisha", "Assam", "Jharkhand", "Chhattisgarh",
                "Uttarakhand", "Goa", "Himachal Pradesh" };

            foreach (var state in knownStates)
            {
                if (text.Contains(state, StringComparison.OrdinalIgnoreCase))
                {
                    guestInfo.State = state;
                    break;
                }
            }
        }

        // Expand state abbreviations to full names
        if (!string.IsNullOrWhiteSpace(guestInfo.State) && guestInfo.State.Length == 2)
        {
            guestInfo.State = ExpandStateAbbreviation(guestInfo.State);
        }

        // Extract Date of Birth (patterns: DOB: DD-MM-YYYY, Date of Birth: DD/MM/YYYY)
        var dobMatch = Regex.Match(text, @"(?:DOB|Date of Birth)[\s:]+(\d{2})[\-\/](\d{2})[\-\/](\d{4})", RegexOptions.IgnoreCase);
        if (dobMatch.Success)
        {
            try
            {
                var day = int.Parse(dobMatch.Groups[1].Value);
                var month = int.Parse(dobMatch.Groups[2].Value);
                var year = int.Parse(dobMatch.Groups[3].Value);
                guestInfo.DateOfBirth = new DateTime(year, month, day);
            }
            catch
            {
                // Invalid date format, skip
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

        // Try to extract Date of Birth
        var dobMatch = Regex.Match(text, @"(?:DOB|Date of Birth|YOB|Year of Birth)[\s:]+(\d{2})[\-\/\.](\d{2})[\-\/\.](\d{4})", RegexOptions.IgnoreCase);
        if (dobMatch.Success)
        {
            try
            {
                var day = int.Parse(dobMatch.Groups[1].Value);
                var month = int.Parse(dobMatch.Groups[2].Value);
                var year = int.Parse(dobMatch.Groups[3].Value);
                guestInfo.DateOfBirth = new DateTime(year, month, day);
            }
            catch
            {
                // Invalid date format, skip
            }
        }
    }

    /// <summary>
    /// Cleans OCR text to keep only English/Latin characters, numbers, and common punctuation
    /// Removes regional language characters (Hindi, Tamil, etc.) while preserving line structure
    /// </summary>
    private string CleanTextToEnglishOnly(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // Process line by line to preserve structure
        var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.None);
        var cleanedLines = new List<string>();

        foreach (var line in lines)
        {
            // Remove non-ASCII characters (regional languages) from each line
            var cleanedLine = Regex.Replace(line, @"[^\x00-\x7F]+", " ");

            // Clean up multiple spaces within the line
            cleanedLine = Regex.Replace(cleanedLine, @"[ \t]+", " ").Trim();

            // Only keep lines that have at least some English letters or numbers
            if (!string.IsNullOrWhiteSpace(cleanedLine) &&
                Regex.IsMatch(cleanedLine, @"[A-Za-z0-9]"))
            {
                cleanedLines.Add(cleanedLine);
            }
        }

        return string.Join("\n", cleanedLines);
    }
}
