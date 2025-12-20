using Agentic_Rentify.Application.Features.Bookings.DTOs;
using MediatR;

namespace Agentic_Rentify.Application.Features.Bookings.Queries.GetActiveBookings;

public class GetActiveBookingsQuery : IRequest<IReadOnlyList<BookingResponseDTO>>
{
}
