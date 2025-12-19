using Agentic_Rentify.Application.Features.Hotels.DTOs;
using Agentic_Rentify.Application.Wrappers;
using MediatR;

namespace Agentic_Rentify.Application.Features.Hotels.Queries.GetAllHotels;

public class GetAllHotelsQuery : IRequest<PagedResponse<HotelResponseDTO>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
}
