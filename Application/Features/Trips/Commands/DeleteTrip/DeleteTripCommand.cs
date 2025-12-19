using MediatR;

namespace Agentic_Rentify.Application.Features.Trips.Commands.DeleteTrip;

public class DeleteTripCommand(int id) : IRequest<int>
{
    public int Id { get; } = id;
}
