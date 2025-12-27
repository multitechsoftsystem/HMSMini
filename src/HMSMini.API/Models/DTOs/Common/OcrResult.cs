namespace HMSMini.API.Models.DTOs.Common;

/// <summary>
/// Result of OCR processing
/// </summary>
public class OcrResult
{
    public bool Success { get; set; }
    public string ExtractedText { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public List<string> Errors { get; set; } = new();
}
