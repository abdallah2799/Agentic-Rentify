using Agentic_Rentify.Application.Features.Trips.DTOs;
using MediatR;

namespace Agentic_Rentify.Application.Features.Trips.Commands.UpdateTrip;

public class UpdateTripCommand : UpdateTripDTO, IRequest<int>
{
}
