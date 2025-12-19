using Agentic_Rentify.Application.Features.Cars.DTOs;
using MediatR;

namespace Agentic_Rentify.Application.Features.Cars.Queries.GetCarById;

public class GetCarByIdQuery(int id) : IRequest<CarResponseDTO>
{
    public int Id { get; } = id;
}
