using MediatR;

namespace Agentic_Rentify.Application.Features.Hotels.Commands.DeleteHotel;

public class DeleteHotelCommand : IRequest<int>
{
    public int Id { get; set; }
}
