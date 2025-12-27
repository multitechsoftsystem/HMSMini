using FluentValidation;
using HMSMini.API.Models.DTOs.CheckIn;

namespace HMSMini.API.Validators;

public class CreateCheckInDtoValidator : AbstractValidator<CreateCheckInDto>
{
    public CreateCheckInDtoValidator()
    {
        RuleFor(x => x.RoomNumber)
            .NotEmpty().WithMessage("Room number is required.")
            .MaximumLength(20).WithMessage("Room number cannot exceed 20 characters.");

        RuleFor(x => x.CheckInDate)
            .NotEmpty().WithMessage("Check-in date is required.");

        RuleFor(x => x.CheckOutDate)
            .NotEmpty().WithMessage("Check-out date is required.")
            .GreaterThan(x => x.CheckInDate).WithMessage("Check-out date must be after check-in date.");

        RuleFor(x => x.Guests)
            .NotEmpty().WithMessage("At least one guest is required.")
            .Must(g => g.Count >= 1 && g.Count <= 3)
            .WithMessage("Number of guests must be between 1 and 3.");

        RuleForEach(x => x.Guests).SetValidator(new CreateGuestDtoValidator());
    }
}
