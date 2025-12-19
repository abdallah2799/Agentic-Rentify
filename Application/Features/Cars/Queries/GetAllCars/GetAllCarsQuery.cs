using Agentic_Rentify.Application.Features.Cars.DTOs;
using Agentic_Rentify.Application.Wrappers;
using MediatR;

namespace Agentic_Rentify.Application.Features.Cars.Queries.GetAllCars;

public class GetAllCarsQuery : IRequest<PagedResponse<CarResponseDTO>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
}
