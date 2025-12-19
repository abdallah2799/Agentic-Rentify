using Agentic_Rentify.Application.Features.Cars.DTOs;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using AutoMapper;
using MediatR;

namespace Agentic_Rentify.Application.Features.Cars.Queries.GetCarById;

public class GetCarByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) 
    : IRequestHandler<GetCarByIdQuery, CarResponseDTO>
{
    public async Task<CarResponseDTO> Handle(GetCarByIdQuery request, CancellationToken cancellationToken)
    {
        var car = await unitOfWork.Repository<Car>().GetByIdAsync(request.Id);
        if (car == null)
        {
            throw new Exception($"Car with ID {request.Id} not found.");
        }

        return mapper.Map<CarResponseDTO>(car);
    }
}
