using FluentValidation;
using HMSMini.API.Models.DTOs.Payment;
using HMSMini.API.Models.Enums;

namespace HMSMini.API.Validators;

public class CreatePaymentDtoValidator : AbstractValidator<CreatePaymentDto>
{
    public CreatePaymentDtoValidator()
    {
        RuleFor(x => x.SourceType)
            .IsInEnum().WithMessage("Invalid source type.");

        RuleFor(x => x.CheckInId)
            .NotNull().WithMessage("CheckInId is required when SourceType is Room.")
            .When(x => x.SourceType == PaymentSourceType.Room);

        RuleFor(x => x.BanquetBookingId)
            .NotNull().WithMessage("BanquetBookingId is required when SourceType is Banquet.")
            .When(x => x.SourceType == PaymentSourceType.Banquet);

        RuleFor(x => x.PaymentDate)
            .NotEmpty().WithMessage("Payment date is required.");

        RuleFor(x => x.PaymentType)
            .IsInEnum().WithMessage("Invalid payment type.");

        RuleFor(x => x.PaymentMode)
            .IsInEnum().WithMessage("Invalid payment mode.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0.");

        RuleFor(x => x.ReferenceNumber)
            .MaximumLength(200).WithMessage("Reference number cannot exceed 200 characters.");

        RuleFor(x => x.ReceivedBy)
            .MaximumLength(100).WithMessage("ReceivedBy cannot exceed 100 characters.");

        RuleFor(x => x.Remarks)
            .MaximumLength(1000).WithMessage("Remarks cannot exceed 1000 characters.");
    }
}
