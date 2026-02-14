using FluentValidation;
using HMSMini.API.Models.DTOs.Company;

namespace HMSMini.API.Validators;

/// <summary>
/// Validator for CreateCompanyDto
/// </summary>
public class CreateCompanyDtoValidator : AbstractValidator<CreateCompanyDto>
{
    public CreateCompanyDtoValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .WithMessage("Company name is required.")
            .MaximumLength(200)
            .WithMessage("Company name cannot exceed 200 characters.");

        RuleFor(x => x.Address)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Address))
            .WithMessage("Address cannot exceed 500 characters.");

        RuleFor(x => x.City)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.City))
            .WithMessage("City cannot exceed 100 characters.");

        RuleFor(x => x.State)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.State))
            .WithMessage("State cannot exceed 100 characters.");

        RuleFor(x => x.Country)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Country))
            .WithMessage("Country cannot exceed 100 characters.");

        RuleFor(x => x.GSTNumber)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.GSTNumber))
            .WithMessage("GST number cannot exceed 50 characters.");

        RuleFor(x => x.ContactPerson)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.ContactPerson))
            .WithMessage("Contact person name cannot exceed 200 characters.");

        RuleFor(x => x.Designation)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Designation))
            .WithMessage("Designation cannot exceed 100 characters.");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Invalid email address.");

        RuleFor(x => x.ContactNumber)
            .Matches(@"^\d{10,15}$")
            .When(x => !string.IsNullOrWhiteSpace(x.ContactNumber))
            .WithMessage("Contact number must be between 10 and 15 digits.");

        RuleFor(x => x.DiscountPercentage)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Discount percentage cannot be negative.")
            .LessThanOrEqualTo(100)
            .WithMessage("Discount percentage cannot exceed 100.");
    }
}
