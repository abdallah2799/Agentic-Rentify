using MediatR;

namespace Agentic_Rentify.Application.Features.Bookings.Commands.ConfirmBooking;

public class ConfirmBookingCommand(string sessionId) : IRequest<Unit>
{
    public string SessionId { get; } = sessionId;
}
