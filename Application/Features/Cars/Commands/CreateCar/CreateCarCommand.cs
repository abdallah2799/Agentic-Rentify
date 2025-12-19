using Agentic_Rentify.Application.Features.Cars.DTOs;
using MediatR;

namespace Agentic_Rentify.Application.Features.Cars.Commands.CreateCar;

public class CreateCarCommand : CreateCarDTO, IRequest<int>
{
}
