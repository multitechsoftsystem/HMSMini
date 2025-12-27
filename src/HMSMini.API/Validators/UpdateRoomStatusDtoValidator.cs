using FluentValidation;
using HMSMini.API.Models.DTOs.Room;
using HMSMini.API.Models.Enums;

namespace HMSMini.API.Validators;

public class UpdateRoomStatusDtoValidator : AbstractValidator<UpdateRoomStatusDto>
{
    public UpdateRoomStatusDtoValidator()
    {
        RuleFor(x => x.RoomStatus)
            .IsInEnum().WithMessage("Invalid room status.");

        RuleFor(x => x.RoomStatusFromDate)
            .NotNull()
            .When(x => x.RoomStatus == RoomStatus.Maintenance || x.RoomStatus == RoomStatus.Blocked)
            .WithMessage("Status from date is required for Maintenance and Blocked statuses.");

        RuleFor(x => x.RoomStatusToDate)
            .NotNull()
            .When(x => x.RoomStatus == RoomStatus.Maintenance || x.RoomStatus == RoomStatus.Blocked)
            .WithMessage("Status to date is required for Maintenance and Blocked statuses.")
            .GreaterThan(x => x.RoomStatusFromDate)
            .When(x => x.RoomStatusFromDate.HasValue)
            .WithMessage("Status to date must be after status from date.");
    }
}
