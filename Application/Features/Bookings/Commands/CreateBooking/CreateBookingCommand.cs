using MediatR;

namespace Agentic_Rentify.Application.Features.Bookings.Commands.CreateBooking;

public class CreateBookingCommand : IRequest<string>
{
    public string UserId { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string BookingType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal TotalPrice { get; set; }
}
