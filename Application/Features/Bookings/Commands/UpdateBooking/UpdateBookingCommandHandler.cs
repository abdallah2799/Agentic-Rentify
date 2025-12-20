using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Core.Entities;
using MediatR;

namespace Agentic_Rentify.Application.Features.Bookings.Commands.UpdateBooking;

public class UpdateBookingCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateBookingCommand, int>
{
    public async Task<int> Handle(UpdateBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await unitOfWork.Repository<Booking>().GetByIdAsync(request.Id);
        if (booking == null || booking.IsDeleted)
        {
            throw new NotFoundException($"Booking with ID {request.Id} not found.");
        }

        booking.StartDate = request.StartDate;
        booking.EndDate = request.EndDate;
        booking.TotalPrice = request.TotalPrice;
        booking.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.Repository<Booking>().UpdateAsync(booking);
        await unitOfWork.CompleteAsync();

        return booking.Id;
    }
}
