using FluentAssertions;
using FluentValidation.TestHelper;
using HMSMini.API.Models.DTOs.Room;
using HMSMini.API.Models.Enums;
using HMSMini.API.Validators;

namespace HMSMini.Tests.Validators;

public class UpdateRoomStatusDtoValidatorTests
{
    private readonly UpdateRoomStatusDtoValidator _validator;

    public UpdateRoomStatusDtoValidatorTests()
    {
        _validator = new UpdateRoomStatusDtoValidator();
    }

    [Fact]
    public async Task Validator_WithAvailableStatus_ShouldNotHaveErrors()
    {
        // Arrange
        var dto = new UpdateRoomStatusDto
        {
            RoomStatus = RoomStatus.Available
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validator_WithMaintenanceAndDates_ShouldNotHaveErrors()
    {
        // Arrange
        var dto = new UpdateRoomStatusDto
        {
            RoomStatus = RoomStatus.Maintenance,
            RoomStatusFromDate = DateTime.Today,
            RoomStatusToDate = DateTime.Today.AddDays(3)
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validator_WithBlockedAndDates_ShouldNotHaveErrors()
    {
        // Arrange
        var dto = new UpdateRoomStatusDto
        {
            RoomStatus = RoomStatus.Blocked,
            RoomStatusFromDate = DateTime.Today,
            RoomStatusToDate = DateTime.Today.AddDays(3)
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validator_WithMaintenanceWithoutFromDate_ShouldHaveError()
    {
        // Arrange
        var dto = new UpdateRoomStatusDto
        {
            RoomStatus = RoomStatus.Maintenance,
            RoomStatusToDate = DateTime.Today.AddDays(3)
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RoomStatusFromDate);
    }

    [Fact]
    public async Task Validator_WithMaintenanceWithoutToDate_ShouldHaveError()
    {
        // Arrange
        var dto = new UpdateRoomStatusDto
        {
            RoomStatus = RoomStatus.Maintenance,
            RoomStatusFromDate = DateTime.Today
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RoomStatusToDate);
    }

    [Fact]
    public async Task Validator_WithBlockedWithoutFromDate_ShouldHaveError()
    {
        // Arrange
        var dto = new UpdateRoomStatusDto
        {
            RoomStatus = RoomStatus.Blocked,
            RoomStatusToDate = DateTime.Today.AddDays(3)
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RoomStatusFromDate);
    }

    [Fact]
    public async Task Validator_WithBlockedWithoutToDate_ShouldHaveError()
    {
        // Arrange
        var dto = new UpdateRoomStatusDto
        {
            RoomStatus = RoomStatus.Blocked,
            RoomStatusFromDate = DateTime.Today
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RoomStatusToDate);
    }

    [Fact]
    public async Task Validator_WithToDateBeforeFromDate_ShouldHaveError()
    {
        // Arrange
        var dto = new UpdateRoomStatusDto
        {
            RoomStatus = RoomStatus.Maintenance,
            RoomStatusFromDate = DateTime.Today.AddDays(3),
            RoomStatusToDate = DateTime.Today
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RoomStatusToDate);
    }
}
