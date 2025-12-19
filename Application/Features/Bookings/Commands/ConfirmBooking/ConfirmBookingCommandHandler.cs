using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Core.Enums;
using MediatR;

namespace Agentic_Rentify.Application.Features.Bookings.Commands.ConfirmBooking;

public class ConfirmBookingCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ConfirmBookingCommand, Unit>
{
    public async Task<Unit> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
    {
        var matches = await unitOfWork.Repository<Booking>()
            .GetAsync(b => b.StripeSessionId == request.SessionId);

        var booking = matches.FirstOrDefault();
        if (booking == null)
        {
            throw new NotFoundException($"Booking with session {request.SessionId} not found.");
        }

        if (booking.IsPaid && booking.Status == BookingStatus.Confirmed)
        {
            return Unit.Value;
        }

        booking.IsPaid = true;
        booking.Status = BookingStatus.Confirmed;
        booking.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.Repository<Booking>().UpdateAsync(booking);
        await unitOfWork.CompleteAsync();

        return Unit.Value;
    }
}
