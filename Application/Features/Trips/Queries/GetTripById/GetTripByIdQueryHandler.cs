using Agentic_Rentify.Application.Features.Trips.DTOs;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using AutoMapper;
using MediatR;

namespace Agentic_Rentify.Application.Features.Trips.Queries.GetTripById;

public class GetTripByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) 
    : IRequestHandler<GetTripByIdQuery, TripResponseDTO>
{
    public async Task<TripResponseDTO> Handle(GetTripByIdQuery request, CancellationToken cancellationToken)
    {
        var trip = await unitOfWork.Repository<Trip>().GetByIdAsync(request.Id);
        if (trip == null)
        {
            throw new Exception($"Trip with ID {request.Id} not found.");
        }

        return mapper.Map<TripResponseDTO>(trip);
    }
}
