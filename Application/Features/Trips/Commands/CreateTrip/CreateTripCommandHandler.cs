using Agentic_Rentify.Application.Features.Trips.DTOs;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using AutoMapper;
using MediatR;

namespace Agentic_Rentify.Application.Features.Trips.Commands.CreateTrip;

public class CreateTripCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) 
    : IRequestHandler<CreateTripCommand, int>
{
    public async Task<int> Handle(CreateTripCommand request, CancellationToken cancellationToken)
    {
        var trip = mapper.Map<Trip>(request);
        await unitOfWork.Repository<Trip>().AddAsync(trip);
        await unitOfWork.CompleteAsync();

        return trip.Id;
    }
}
