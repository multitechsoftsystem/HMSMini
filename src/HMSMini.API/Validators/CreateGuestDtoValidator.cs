using FluentValidation;
using HMSMini.API.Models.DTOs.Guest;

namespace HMSMini.API.Validators;

public class CreateGuestDtoValidator : AbstractValidator<CreateGuestDto>
{
    public CreateGuestDtoValidator()
    {
        RuleFor(x => x.GuestName)
            .NotEmpty().WithMessage("Guest name is required.")
            .MaximumLength(200).WithMessage("Guest name cannot exceed 200 characters.");

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Address cannot exceed 500 characters.");

        RuleFor(x => x.City)
            .MaximumLength(100).WithMessage("City cannot exceed 100 characters.");

        RuleFor(x => x.State)
            .MaximumLength(100).WithMessage("State cannot exceed 100 characters.");

        RuleFor(x => x.Country)
            .MaximumLength(100).WithMessage("Country cannot exceed 100 characters.");

        RuleFor(x => x.MobileNo)
            .MaximumLength(20).WithMessage("Mobile number cannot exceed 20 characters.")
            .Matches(@"^\d{10}$").When(x => !string.IsNullOrEmpty(x.MobileNo))
            .WithMessage("Mobile number must be 10 digits.");
    }
}
