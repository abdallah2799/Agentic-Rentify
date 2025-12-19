using MediatR;

namespace Agentic_Rentify.Application.Features.Attractions.Commands.DeleteAttraction;

public class DeleteAttractionCommand : IRequest<bool>
{
    public int Id { get; set; }

    public DeleteAttractionCommand(int id)
    {
        Id = id;
    }
}
