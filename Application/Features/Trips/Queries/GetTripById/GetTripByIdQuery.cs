using Agentic_Rentify.Application.Features.Trips.DTOs;
using MediatR;

namespace Agentic_Rentify.Application.Features.Trips.Queries.GetTripById;

public class GetTripByIdQuery(int id) : IRequest<TripResponseDTO>
{
    public int Id { get; } = id;
}
