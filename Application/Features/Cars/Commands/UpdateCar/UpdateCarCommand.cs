using Agentic_Rentify.Application.Features.Cars.DTOs;
using MediatR;

namespace Agentic_Rentify.Application.Features.Cars.Commands.UpdateCar;

public class UpdateCarCommand : UpdateCarDTO, IRequest<int>
{
}
