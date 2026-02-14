using FluentValidation;
using HMSMini.API.Models.DTOs.BusinessSource;

namespace HMSMini.API.Validators;

/// <summary>
/// Validator for UpdateBusinessSourceDto
/// </summary>
public class UpdateBusinessSourceDtoValidator : AbstractValidator<UpdateBusinessSourceDto>
{
    public UpdateBusinessSourceDtoValidator()
    {
        RuleFor(x => x.SourceName)
            .NotEmpty()
            .WithMessage("Source name is required.")
            .MaximumLength(100)
            .WithMessage("Source name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Description))
            .WithMessage("Description cannot exceed 500 characters.");
    }
}
