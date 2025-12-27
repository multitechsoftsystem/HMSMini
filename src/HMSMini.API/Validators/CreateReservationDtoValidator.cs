using FluentValidation;
using HMSMini.API.Models.DTOs.Reservation;

namespace HMSMini.API.Validators;

/// <summary>
/// Validator for CreateReservationDto
/// </summary>
public class CreateReservationDtoValidator : AbstractValidator<CreateReservationDto>
{
    public CreateReservationDtoValidator()
    {
        RuleFor(x => x.RoomNumber)
            .NotEmpty()
            .WithMessage("Room number is required.")
            .MaximumLength(20)
            .WithMessage("Room number cannot exceed 20 characters.");

        RuleFor(x => x.CheckInDate)
            .NotEmpty()
            .WithMessage("Check-in date is required.");

        RuleFor(x => x.CheckOutDate)
            .NotEmpty()
            .WithMessage("Check-out date is required.")
            .GreaterThan(x => x.CheckInDate)
            .WithMessage("Check-out date must be after check-in date.");

        RuleFor(x => x.NumberOfGuests)
            .GreaterThanOrEqualTo(1)
            .WithMessage("At least one guest is required.")
            .LessThanOrEqualTo(3)
            .WithMessage("Maximum 3 guests allowed per room.");

        RuleFor(x => x.GuestName)
            .NotEmpty()
            .WithMessage("Guest name is required.")
            .MaximumLength(200)
            .WithMessage("Guest name cannot exceed 200 characters.");

        RuleFor(x => x.GuestEmail)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.GuestEmail))
            .WithMessage("Invalid email address.");

        RuleFor(x => x.GuestMobile)
            .NotEmpty()
            .WithMessage("Guest mobile number is required.")
            .Matches(@"^\d{10,15}$")
            .WithMessage("Mobile number must be between 10 and 15 digits.");

        RuleFor(x => x.SpecialRequests)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.SpecialRequests))
            .WithMessage("Special requests cannot exceed 1000 characters.");
    }
}
