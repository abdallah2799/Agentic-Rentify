using Agentic_Rentify.Application.Features.Trips.DTOs;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using AutoMapper;
using MediatR;

namespace Agentic_Rentify.Application.Features.Trips.Commands.UpdateTrip;

public class UpdateTripCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) 
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

        return trip.Id;
    }
}
