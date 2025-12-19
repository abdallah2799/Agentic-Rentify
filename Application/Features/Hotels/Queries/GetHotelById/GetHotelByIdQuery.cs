using Agentic_Rentify.Application.Features.Hotels.DTOs;
using MediatR;

namespace Agentic_Rentify.Application.Features.Hotels.Queries.GetHotelById;

public class GetHotelByIdQuery(int id) : IRequest<HotelResponseDTO>
{
    public int Id { get; } = id;
}
