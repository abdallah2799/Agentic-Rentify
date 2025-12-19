using Agentic_Rentify.Application.Features.Attractions.DTOs;
using MediatR;

namespace Agentic_Rentify.Application.Features.Attractions.Queries.GetAllAttractions;

public class GetAllAttractionsQuery : IRequest<IReadOnlyList<AttractionResponseDTO>>
{
}
