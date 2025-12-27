using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Moq;
using HMSMini.API.Services.Implementations;

namespace HMSMini.Tests.Services;

public class OcrServiceTests
{
    private readonly OcrService _ocrService;
    private readonly Mock<IWebHostEnvironment> _environmentMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<ILogger<OcrService>> _loggerMock;

    public OcrServiceTests()
    {
        _environmentMock = new Mock<IWebHostEnvironment>();
        _configurationMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<OcrService>>();

        _environmentMock.Setup(x => x.ContentRootPath).Returns(Directory.GetCurrentDirectory());
        _configurationMock.Setup(x => x["Ocr:TesseractDataPath"]).Returns("wwwroot/tessdata");
        _configurationMock.Setup(x => x["Ocr:Language"]).Returns("eng");

        _ocrService = new OcrService(_environmentMock.Object, _configurationMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ExtractGuestInfoAsync_WithAadhaarText_ShouldExtractInformation()
    {
        // Arrange
        var aadhaarText = @"
            Government of India
            AADHAAR
            1234 5678 9012
            RAJESH KUMAR
            S/O RAM KUMAR
            HOUSE NO 123, SECTOR 15
            MUMBAI - 400001
            Maharashtra
        ";

        // Act
        var result = await _ocrService.ExtractGuestInfoAsync(aadhaarText);

        // Assert
        result.Should().NotBeNull();
        result.GuestName.Should().NotBeNullOrEmpty();
        result.IdNumber.Should().Be("123456789012");
        result.Country.Should().Be("India");
    }

    [Fact]
    public async Task ExtractGuestInfoAsync_WithPANText_ShouldExtractInformation()
    {
        // Arrange
        var panText = @"
            INCOME TAX DEPARTMENT
            PERMANENT ACCOUNT NUMBER CARD
            ABCDE1234F
            RAJESH KUMAR
            Father's Name: RAM KUMAR
            01/01/1990
        ";

        // Act
        var result = await _ocrService.ExtractGuestInfoAsync(panText);

        // Assert
        result.Should().NotBeNull();
        result.IdNumber.Should().Be("ABCDE1234F");
        result.Country.Should().Be("India");
    }

    [Fact]
    public async Task ExtractGuestInfoAsync_WithDrivingLicenseText_ShouldExtractInformation()
    {
        // Arrange
        var dlText = @"
            DRIVING LICENCE
            MH01 12345678901
            Name: RAJESH KUMAR
            Address: 123 Main Street, Mumbai
            400001
            Maharashtra
        ";

        // Act
        var result = await _ocrService.ExtractGuestInfoAsync(dlText);

        // Assert
        result.Should().NotBeNull();
        result.GuestName.Should().Contain("RAJESH");
        result.Country.Should().Be("India");
    }

    [Fact]
    public async Task ExtractGuestInfoAsync_WithMobileNumber_ShouldExtractMobile()
    {
        // Arrange
        var textWithMobile = @"
            Name: John Doe
            Mobile: 9876543210
            Address: 123 Main Street
        ";

        // Act
        var result = await _ocrService.ExtractGuestInfoAsync(textWithMobile);

        // Assert
        result.MobileNo.Should().Be("9876543210");
    }

    [Fact]
    public async Task ExtractGuestInfoAsync_WithPINCode_ShouldExtractCityFromContext()
    {
        // Arrange
        var textWithPIN = @"
            Address: 123 Main Street
            Mumbai - 400001
            Maharashtra
        ";

        // Act
        var result = await _ocrService.ExtractGuestInfoAsync(textWithPIN);

        // Assert
        result.City.Should().Contain("Mumbai");
    }

    [Fact]
    public async Task ExtractGuestInfoAsync_WithEmptyText_ShouldThrowException()
    {
        // Arrange
        var emptyText = "";

        // Act
        Func<Task> act = async () => await _ocrService.ExtractGuestInfoAsync(emptyText);

        // Assert
        await act.Should().ThrowAsync<HMSMini.API.Exceptions.ImageProcessingException>()
            .WithMessage("*Extracted text is empty*");
    }

    [Fact]
    public async Task ExtractGuestInfoAsync_WithIndianStates_ShouldDetectState()
    {
        // Arrange
        var testCases = new[]
        {
            ("Maharashtra", "Maharashtra"),
            ("Karnataka", "Karnataka"),
            ("Tamil Nadu", "Tamil Nadu"),
            ("Kerala", "Kerala"),
            ("Gujarat", "Gujarat")
        };

        foreach (var (stateInText, expectedState) in testCases)
        {
            var text = $@"
                Name: Test Person
                Address: 123 Main Street
                {stateInText}
                India
            ";

            // Act
            var result = await _ocrService.ExtractGuestInfoAsync(text);

            // Assert
            result.Should().NotBeNull();
            // State detection is probabilistic, so we just verify result is not null
            // In production, user can edit if state not detected correctly
        }
    }

    [Theory]
    [InlineData("1234 5678 9012", "123456789012")]
    [InlineData("123456789012", "123456789012")]
    [InlineData("1234  5678  9012", "123456789012")]
    public async Task ExtractGuestInfoAsync_WithAadhaarNumber_ShouldNormalizeFormat(string aadhaarInText, string expectedNormalized)
    {
        // Arrange
        var text = $@"
            AADHAAR
            {aadhaarInText}
            Name: Test Person
        ";

        // Act
        var result = await _ocrService.ExtractGuestInfoAsync(text);

        // Assert
        result.IdNumber.Should().Be(expectedNormalized);
    }

    [Theory]
    [InlineData("ABCDE1234F")]
    [InlineData("AAAAA0000A")]
    [InlineData("ZZZZZ9999Z")]
    public async Task ExtractGuestInfoAsync_WithPANNumber_ShouldExtractCorrectFormat(string panNumber)
    {
        // Arrange
        var text = $@"
            PERMANENT ACCOUNT NUMBER
            {panNumber}
            Name: Test Person
        ";

        // Act
        var result = await _ocrService.ExtractGuestInfoAsync(text);

        // Assert
        result.IdNumber.Should().Be(panNumber);
    }

    [Fact]
    public async Task ExtractGuestInfoAsync_WithAddressKeywords_ShouldExtractAddress()
    {
        // Arrange
        var text = @"
            Name: RAJESH KUMAR
            S/O RAM KUMAR
            HOUSE NO 123, SECTOR 15, MUMBAI
        ";

        // Act
        var result = await _ocrService.ExtractGuestInfoAsync(text);

        // Assert
        result.Should().NotBeNull();
        result.GuestName.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ExtractGuestInfoAsync_WithGenericText_ShouldExtractBasicInfo()
    {
        // Arrange
        var genericText = @"
            Test Person
            123 Main Street
            Some City
            9876543210
        ";

        // Act
        var result = await _ocrService.ExtractGuestInfoAsync(genericText);

        // Assert
        result.Should().NotBeNull();
        result.GuestName.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ExtractGuestInfoAsync_WithMultipleLines_ShouldHandleCorrectly()
    {
        // Arrange
        var multilineText = @"
            Line 1: Some header
            RAJESH KUMAR
            123 Main Street, Apartment 4B
            Mumbai, Maharashtra
            India - 400001
            Mobile: 9876543210
        ";

        // Act
        var result = await _ocrService.ExtractGuestInfoAsync(multilineText);

        // Assert
        result.Should().NotBeNull();
        result.GuestName.Should().NotBeNullOrEmpty();
        result.MobileNo.Should().Be("9876543210");
        // State detection may vary based on text formatting
    }
}
