using FluentValidation;

namespace Agentic_Rentify.Application.Features.Bookings.Commands.UpdateBooking;

public class UpdateBookingCommandValidator : AbstractValidator<UpdateBookingCommand>
{
    public UpdateBookingCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.EndDate.HasValue)
            .WithMessage("EndDate must be on or after StartDate.");
        RuleFor(x => x.TotalPrice).GreaterThan(0);
    }
}
