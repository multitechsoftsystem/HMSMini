using FluentValidation;
using HMSMini.API.Models.DTOs.MealPlan;

namespace HMSMini.API.Validators;

/// <summary>
/// Validator for CreateMealPlanRateDto
/// </summary>
public class CreateMealPlanRateDtoValidator : AbstractValidator<CreateMealPlanRateDto>
{
    public CreateMealPlanRateDtoValidator()
    {
        RuleFor(x => x.MealPlanId)
            .GreaterThan(0)
            .WithMessage("Meal plan ID must be greater than 0.");

        RuleFor(x => x.RoomTypeId)
            .GreaterThan(0)
            .WithMessage("Room type ID must be greater than 0.");

        RuleFor(x => x.RatePerPersonPerNight)
            .GreaterThan(0)
            .WithMessage("Rate per person per night must be greater than 0.")
            .LessThan(100000)
            .WithMessage("Rate per person per night must be less than 100000.");

        RuleFor(x => x.EffectiveFrom)
            .NotEmpty()
            .WithMessage("Effective from date is required.");

        RuleFor(x => x.EffectiveTo)
            .GreaterThan(x => x.EffectiveFrom)
            .When(x => x.EffectiveTo.HasValue)
            .WithMessage("Effective to date must be after effective from date.");
    }
}
