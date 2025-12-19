using FluentValidation;

namespace Agentic_Rentify.Application.Features.Bookings.Commands.CreateBooking;

public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    private static readonly HashSet<string> AllowedBookingTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "Trip",
        "Hotel",
        "Car",
        "Attraction"
    };

    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.EntityId)
            .GreaterThan(0).WithMessage("EntityId must be greater than zero.");

        RuleFor(x => x.BookingType)
            .NotEmpty().WithMessage("BookingType is required.")
            .Must(type => AllowedBookingTypes.Contains(type))
            .WithMessage($"BookingType must be one of: {string.Join(", ", AllowedBookingTypes)}.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("StartDate is required.");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.EndDate.HasValue)
            .WithMessage("EndDate must be on or after StartDate.");

        RuleFor(x => x.TotalPrice)
            .GreaterThan(0).WithMessage("TotalPrice must be greater than zero.");
    }
}
