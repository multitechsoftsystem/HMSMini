namespace HMSMini.API.Models.DTOs.Guest;

/// <summary>
/// OCR review data including raw text and parsed guest information
/// </summary>
public class OcrReviewDto
{
    /// <summary>
    /// Raw text extracted from OCR
    /// </summary>
    public string RawText { get; set; } = string.Empty;

    /// <summary>
    /// OCR confidence score (0-1)
    /// </summary>
    public double Confidence { get; set; }

    /// <summary>
    /// Parsed guest information from the extracted text
    /// </summary>
    public GuestInfoDto GuestInfo { get; set; } = new();
}
