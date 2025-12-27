namespace HMSMini.API.Models.DTOs.Common;

/// <summary>
/// Error response for API
/// </summary>
public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public List<string>? Errors { get; set; }
    public string? StackTrace { get; set; } // Only in development
}
