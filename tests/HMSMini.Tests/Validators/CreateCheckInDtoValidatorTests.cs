using FluentAssertions;
using FluentValidation.TestHelper;
using HMSMini.API.Models.DTOs.CheckIn;
using HMSMini.API.Models.DTOs.Guest;
using HMSMini.API.Validators;

namespace HMSMini.Tests.Validators;

public class CreateCheckInDtoValidatorTests
{
    private readonly CreateCheckInDtoValidator _validator;

    public CreateCheckInDtoValidatorTests()
    {
        _validator = new CreateCheckInDtoValidator();
    }

    [Fact]
    public async Task Validator_WithValidData_ShouldNotHaveErrors()
    {
        // Arrange
        var dto = new CreateCheckInDto
        {
            RoomNumber = "101",
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            Guests = new List<CreateGuestDto>
            {
                new CreateGuestDto
                {
                    GuestName = "John Doe",
                    MobileNo = "1234567890"
                }
            }
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validator_WithMissingRoomNumber_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateCheckInDto
        {
            RoomNumber = "",
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            Guests = new List<CreateGuestDto>
            {
                new CreateGuestDto { GuestName = "John Doe", MobileNo = "1234567890" }
            }
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RoomNumber);
    }

    [Fact]
    public async Task Validator_WithCheckOutDateBeforeCheckIn_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateCheckInDto
        {
            RoomNumber = "101",
            CheckInDate = DateTime.Today.AddDays(2),
            CheckOutDate = DateTime.Today,
            Guests = new List<CreateGuestDto>
            {
                new CreateGuestDto { GuestName = "John Doe", MobileNo = "1234567890" }
            }
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CheckOutDate);
    }

    [Fact]
    public async Task Validator_WithNoGuests_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateCheckInDto
        {
            RoomNumber = "101",
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            Guests = new List<CreateGuestDto>()
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Guests);
    }

    [Fact]
    public async Task Validator_WithMoreThan3Guests_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateCheckInDto
        {
            RoomNumber = "101",
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            Guests = new List<CreateGuestDto>
            {
                new CreateGuestDto { GuestName = "Guest 1", MobileNo = "1234567890" },
                new CreateGuestDto { GuestName = "Guest 2", MobileNo = "1234567890" },
                new CreateGuestDto { GuestName = "Guest 3", MobileNo = "1234567890" },
                new CreateGuestDto { GuestName = "Guest 4", MobileNo = "1234567890" }
            }
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Guests);
    }
}
