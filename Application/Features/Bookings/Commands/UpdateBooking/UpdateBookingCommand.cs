using MediatR;

namespace Agentic_Rentify.Application.Features.Bookings.Commands.UpdateBooking;

public class UpdateBookingCommand : IRequest<int>
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal TotalPrice { get; set; }
}
