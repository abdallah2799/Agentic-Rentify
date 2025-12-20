using Agentic_Rentify.Application.Features.Bookings.DTOs;
using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Core.Enums;
using MediatR;

namespace Agentic_Rentify.Application.Features.Bookings.Queries.GetActiveBookings;

public class GetActiveBookingsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetActiveBookingsQuery, IReadOnlyList<BookingResponseDTO>>
{
    public async Task<IReadOnlyList<BookingResponseDTO>> Handle(GetActiveBookingsQuery request, CancellationToken cancellationToken)
    {
        var items = await unitOfWork.Repository<Booking>()
            .GetAsync(b => !b.IsDeleted && b.Status != BookingStatus.Cancelled && b.Status != BookingStatus.Failed);

        return items
            .Select(b => new BookingResponseDTO
            {
                Id = b.Id,
                UserId = b.UserId,
                EntityId = b.EntityId,
                BookingType = b.BookingType,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                TotalPrice = b.TotalPrice,
                Status = b.Status,
                IsPaid = b.IsPaid
            })
            .ToList();
    }
}
