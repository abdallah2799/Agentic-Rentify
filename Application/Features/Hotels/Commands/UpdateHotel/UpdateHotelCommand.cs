using Agentic_Rentify.Application.Features.Hotels.DTOs;
using MediatR;

namespace Agentic_Rentify.Application.Features.Hotels.Commands.UpdateHotel;

public class UpdateHotelCommand : UpdateHotelDTO, IRequest<int>
{
}
