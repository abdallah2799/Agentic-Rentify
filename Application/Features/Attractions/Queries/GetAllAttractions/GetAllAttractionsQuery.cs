using Agentic_Rentify.Application.Features.Attractions.DTOs;
using Agentic_Rentify.Application.Wrappers;
using MediatR;

namespace Agentic_Rentify.Application.Features.Attractions.Queries.GetAllAttractions;

public class GetAllAttractionsQuery : IRequest<PagedResponse<AttractionResponseDTO>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
}
