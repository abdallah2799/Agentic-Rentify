using MediatR;

namespace Agentic_Rentify.Application.Features.Bookings.Commands.DeleteBooking;

public class DeleteBookingCommand(int id) : IRequest<Unit>
{
    public int Id { get; } = id;
}
