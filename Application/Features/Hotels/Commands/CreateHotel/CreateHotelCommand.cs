using Agentic_Rentify.Application.Features.Hotels.DTOs;
using MediatR;

namespace Agentic_Rentify.Application.Features.Hotels.Commands.CreateHotel;

public class CreateHotelCommand : CreateHotelDTO, IRequest<int>
{
}
