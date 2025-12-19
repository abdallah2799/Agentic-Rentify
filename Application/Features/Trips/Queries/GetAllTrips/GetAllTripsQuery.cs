using Agentic_Rentify.Application.Features.Trips.DTOs;
using Agentic_Rentify.Application.Wrappers;
using MediatR;

namespace Agentic_Rentify.Application.Features.Trips.Queries.GetAllTrips;

public class GetAllTripsQuery : IRequest<PagedResponse<TripResponseDTO>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
}
