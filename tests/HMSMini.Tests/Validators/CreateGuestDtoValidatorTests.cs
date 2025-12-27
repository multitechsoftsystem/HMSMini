using FluentAssertions;
using FluentValidation.TestHelper;
using HMSMini.API.Models.DTOs.Guest;
using HMSMini.API.Validators;

namespace HMSMini.Tests.Validators;

public class CreateGuestDtoValidatorTests
{
    private readonly CreateGuestDtoValidator _validator;

    public CreateGuestDtoValidatorTests()
    {
        _validator = new CreateGuestDtoValidator();
    }

    [Fact]
    public async Task Validator_WithValidData_ShouldNotHaveErrors()
    {
        // Arrange
        var dto = new CreateGuestDto
        {
            GuestName = "John Doe",
            Address = "123 Main St",
            City = "New York",
            State = "NY",
            Country = "USA",
            MobileNo = "1234567890"
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validator_WithMissingGuestName_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateGuestDto
        {
            GuestName = "",
            MobileNo = "1234567890"
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.GuestName);
    }

    [Fact]
    public async Task Validator_WithGuestNameTooLong_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateGuestDto
        {
            GuestName = new string('A', 201), // 201 characters
            MobileNo = "1234567890"
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.GuestName);
    }

    [Fact]
    public async Task Validator_WithInvalidMobileNoFormat_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateGuestDto
        {
            GuestName = "John Doe",
            MobileNo = "123" // Too short
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MobileNo);
    }

    [Fact]
    public async Task Validator_WithValidMobileNo10Digits_ShouldNotHaveError()
    {
        // Arrange
        var dto = new CreateGuestDto
        {
            GuestName = "John Doe",
            MobileNo = "1234567890"
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.MobileNo);
    }

    [Fact]
    public async Task Validator_WithCityTooLong_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateGuestDto
        {
            GuestName = "John Doe",
            City = new string('A', 101), // 101 characters
            MobileNo = "1234567890"
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.City);
    }

    [Fact]
    public async Task Validator_WithStateTooLong_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateGuestDto
        {
            GuestName = "John Doe",
            State = new string('A', 101), // 101 characters
            MobileNo = "1234567890"
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.State);
    }

    [Fact]
    public async Task Validator_WithCountryTooLong_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateGuestDto
        {
            GuestName = "John Doe",
            Country = new string('A', 101), // 101 characters
            MobileNo = "1234567890"
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Country);
    }

    [Fact]
    public async Task Validator_WithAddressTooLong_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateGuestDto
        {
            GuestName = "John Doe",
            Address = new string('A', 501), // 501 characters
            MobileNo = "1234567890"
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Address);
    }
}
