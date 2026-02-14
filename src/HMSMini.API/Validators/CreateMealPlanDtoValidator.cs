using FluentValidation;
using HMSMini.API.Models.DTOs.MealPlan;

namespace HMSMini.API.Validators;

/// <summary>
/// Validator for CreateMealPlanDto
/// </summary>
public class CreateMealPlanDtoValidator : AbstractValidator<CreateMealPlanDto>
{
    public CreateMealPlanDtoValidator()
    {
        RuleFor(x => x.PlanCode)
            .NotEmpty()
            .WithMessage("Plan code is required.")
            .MaximumLength(10)
            .WithMessage("Plan code cannot exceed 10 characters.")
            .Matches("^[A-Z]{2,10}$")
            .WithMessage("Plan code must contain only uppercase letters (2-10 characters).");

        RuleFor(x => x.PlanName)
            .NotEmpty()
            .WithMessage("Plan name is required.")
            .MaximumLength(100)
            .WithMessage("Plan name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Description))
            .WithMessage("Description cannot exceed 500 characters.");
    }
}
