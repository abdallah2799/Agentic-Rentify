using Agentic_Rentify.Application.Features.Attractions.DTOs;
using MediatR;

namespace Agentic_Rentify.Application.Features.Attractions.Queries.GetAttractionById;

public class GetAttractionByIdQuery : IRequest<AttractionResponseDTO>
{
    public int Id { get; set; }

    public GetAttractionByIdQuery(int id)
    {
        Id = id;
    }
}
