using Agentic_Rentify.Application.Features.Attractions.DTOs;
using Agentic_Rentify.Application.Wrappers;
using MediatR;

namespace Agentic_Rentify.Application.Features.Attractions.Queries.GetAllAttractions;

public class GetAllAttractionsQuery : IRequest<PagedResponse<AttractionResponseDTO>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public string? City { get; set; }
    public string? Category { get; set; }
    public double? MinRating { get; set; }
}
