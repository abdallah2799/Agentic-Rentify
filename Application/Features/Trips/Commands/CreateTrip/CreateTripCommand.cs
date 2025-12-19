using Agentic_Rentify.Application.Features.Trips.DTOs;
using MediatR;

namespace Agentic_Rentify.Application.Features.Trips.Commands.CreateTrip;

public class CreateTripCommand : CreateTripDTO, IRequest<int>
{
}
