using Agentic_Rentify.Application.Features.Bookings.DTOs;
using MediatR;

namespace Agentic_Rentify.Application.Features.Bookings.Queries.GetBookingById;

public class GetBookingByIdQuery(int id) : IRequest<BookingResponseDTO>
{
    public int Id { get; } = id;
}
