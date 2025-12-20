using MediatR;

namespace Agentic_Rentify.Application.Features.Bookings.Commands.CancelBooking;

public class CancelBookingCommand(int id) : IRequest<Unit>
{
    public int Id { get; } = id;
}
