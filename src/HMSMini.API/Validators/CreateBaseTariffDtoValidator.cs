using FluentValidation;
using HMSMini.API.Models.DTOs.Tariff;

namespace HMSMini.API.Validators;

/// <summary>
/// Validator for CreateBaseTariffDto
/// </summary>
public class CreateBaseTariffDtoValidator : AbstractValidator<CreateBaseTariffDto>
{
    public CreateBaseTariffDtoValidator()
    {
        RuleFor(x => x.RoomTypeId)
            .GreaterThan(0)
            .WithMessage("Room type ID must be greater than 0.");

        RuleFor(x => x.OccupancyCount)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Occupancy count must be at least 1.")
            .LessThanOrEqualTo(6)
            .WithMessage("Occupancy count cannot exceed 6.");

        RuleFor(x => x.RatePerNight)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Rate per night cannot be negative.");

        RuleFor(x => x.EffectiveFrom)
            .NotEmpty()
            .WithMessage("Effective from date is required.");

        RuleFor(x => x.EffectiveTo)
            .GreaterThan(x => x.EffectiveFrom)
            .When(x => x.EffectiveTo.HasValue)
            .WithMessage("Effective to date must be after effective from date.");
    }
}
