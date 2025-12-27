namespace HMSMini.API.Exceptions;

/// <summary>
/// Exception thrown when image processing fails
/// </summary>
public class ImageProcessingException : Exception
{
    public ImageProcessingException(string message) : base(message)
    {
    }

    public ImageProcessingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
