using Agentic_Rentify.Application.Features.Bookings.DTOs;
using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Core.Entities;
using MediatR;

namespace Agentic_Rentify.Application.Features.Bookings.Queries.GetBookingById;

public class GetBookingByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetBookingByIdQuery, BookingResponseDTO>
{
    public async Task<BookingResponseDTO> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await unitOfWork.Repository<Booking>().GetByIdAsync(request.Id);
        if (entity == null || entity.IsDeleted)
        {
            throw new NotFoundException($"Booking with ID {request.Id} not found.");
        }

        return new BookingResponseDTO
        {
            Id = entity.Id,
            UserId = entity.UserId,
            EntityId = entity.EntityId,
            BookingType = entity.BookingType,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            TotalPrice = entity.TotalPrice,
            Status = entity.Status,
            IsPaid = entity.IsPaid
        };
    }
}
