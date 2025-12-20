using Agentic_Rentify.Application.Features.Trips.DTOs;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using AutoMapper;
using MediatR;
using Agentic_Rentify.Application.Features.SyncVector;

namespace Agentic_Rentify.Application.Features.Trips.Commands.CreateTrip;

public class CreateTripCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator) 
    : IRequestHandler<CreateTripCommand, int>
{
    public async Task<int> Handle(CreateTripCommand request, CancellationToken cancellationToken)
    {
        var trip = mapper.Map<Trip>(request);
        await unitOfWork.Repository<Trip>().AddAsync(trip);
        await unitOfWork.CompleteAsync();

        var text = string.Join(" ", new[] { trip.Title, trip.Description });
        await mediator.Publish(new EntitySavedToVectorDbEvent(
            trip.Id,
            "Trip",
            text,
            name: trip.Title,
            price: trip.Price,
            city: null), cancellationToken);

        return trip.Id;
    }
}
