using Agentic_Rentify.Application.Features.Trips.DTOs;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using AutoMapper;
using MediatR;
using Agentic_Rentify.Application.Features.SyncVector;

namespace Agentic_Rentify.Application.Features.Trips.Commands.UpdateTrip;

public class UpdateTripCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator) 
    : IRequestHandler<UpdateTripCommand, int>
{
    public async Task<int> Handle(UpdateTripCommand request, CancellationToken cancellationToken)
    {
        var trip = await unitOfWork.Repository<Trip>().GetByIdAsync(request.Id);
        if (trip == null)
        {
            throw new Exception($"Trip with ID {request.Id} not found.");
        }

        mapper.Map(request, trip);
        await unitOfWork.Repository<Trip>().UpdateAsync(trip);
        await unitOfWork.CompleteAsync();

        var text = string.Join(" ", new[] { trip.Title, trip.Description, trip.City });
        await mediator.Publish(new EntitySavedToVectorDbEvent(
            trip.Id,
            "Trip",
            text,
            name: trip.Title,
            price: trip.Price,
            city: trip.City), cancellationToken);

        return trip.Id;
    }
}
