using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Core.Enums;
using MediatR;

namespace Agentic_Rentify.Application.Features.Bookings.Commands.DeleteBooking;

public class DeleteBookingCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteBookingCommand, Unit>
{
    public async Task<Unit> Handle(DeleteBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await unitOfWork.Repository<Booking>().GetByIdAsync(request.Id);
        if (booking == null || booking.IsDeleted)
        {
            throw new NotFoundException($"Booking with ID {request.Id} not found.");
        }

        // Soft delete to preserve audit trail; mark cancelled if not already
        booking.IsDeleted = true;
        if (booking.Status != BookingStatus.Cancelled)
        {
            booking.Status = BookingStatus.Cancelled;
        }
        booking.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.Repository<Booking>().UpdateAsync(booking);
        await unitOfWork.CompleteAsync();

        return Unit.Value;
    }
}
