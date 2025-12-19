using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using MediatR;

namespace Agentic_Rentify.Application.Features.Trips.Commands.DeleteTrip;

public class DeleteTripCommandHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<DeleteTripCommand, int>
{
    public async Task<int> Handle(DeleteTripCommand request, CancellationToken cancellationToken)
    {
        var trip = await unitOfWork.Repository<Trip>().GetByIdAsync(request.Id);
        if (trip == null)
        {
            throw new Exception($"Trip with ID {request.Id} not found.");
        }

        await unitOfWork.Repository<Trip>().DeleteAsync(trip);
        await unitOfWork.CompleteAsync();

        return trip.Id;
    }
}
