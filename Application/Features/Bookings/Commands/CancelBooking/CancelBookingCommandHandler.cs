using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Core.Enums;
using MediatR;

namespace Agentic_Rentify.Application.Features.Bookings.Commands.CancelBooking;

public class CancelBookingCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CancelBookingCommand, Unit>
{
    public async Task<Unit> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await unitOfWork.Repository<Booking>().GetByIdAsync(request.Id);
        if (booking == null || booking.IsDeleted)
        {
            throw new NotFoundException($"Booking with ID {request.Id} not found.");
        }

        booking.Status = BookingStatus.Cancelled;
        booking.UpdatedAt = DateTime.UtcNow;

        // NOTE: If refunds are needed, integrate Stripe Refunds here.

        await unitOfWork.Repository<Booking>().UpdateAsync(booking);
        await unitOfWork.CompleteAsync();

        return Unit.Value;
    }
}
