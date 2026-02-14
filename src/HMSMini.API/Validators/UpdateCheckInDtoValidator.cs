using FluentValidation;
using HMSMini.API.Models.DTOs.CheckIn;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Validators;

public class UpdateCheckInDtoValidator : AbstractValidator<UpdateCheckInDto>
{
    private readonly ISystemSettingsService _systemSettingsService;

    public UpdateCheckInDtoValidator(ISystemSettingsService systemSettingsService)
    {
        _systemSettingsService = systemSettingsService;

        // All fields are optional, but if provided they must be valid
        When(x => x.CompanyId.HasValue, () =>
        {
            RuleFor(x => x.CompanyId)
                .GreaterThan(0)
                .WithMessage("Company ID must be greater than 0");
        });

        When(x => x.BusinessSourceId.HasValue, () =>
        {
            RuleFor(x => x.BusinessSourceId)
                .GreaterThan(0)
                .WithMessage("Business Source ID must be greater than 0");
        });

        When(x => x.MealPlanId.HasValue, () =>
        {
            RuleFor(x => x.MealPlanId)
                .GreaterThan(0)
                .WithMessage("Meal Plan ID must be greater than 0");
        });

        When(x => !string.IsNullOrWhiteSpace(x.Remarks), () =>
        {
            RuleFor(x => x.Remarks)
                .MaximumLength(500)
                .WithMessage("Remarks cannot exceed 500 characters");
        });

        // CheckOut date validation
        When(x => x.CheckOutDate.HasValue, () =>
        {
            RuleFor(x => x.CheckOutDate!.Value)
                .MustAsync(async (date, cancellation) =>
                {
                    var workingDate = await _systemSettingsService.GetWorkingDateAsync();
                    return date >= workingDate;
                })
                .WithMessage("Checkout date cannot be before the working date. Past dates are already closed.");
        });
    }
}
