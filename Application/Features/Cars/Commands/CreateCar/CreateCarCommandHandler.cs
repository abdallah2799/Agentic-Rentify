using Agentic_Rentify.Application.Features.Cars.DTOs;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using AutoMapper;
using MediatR;

namespace Agentic_Rentify.Application.Features.Cars.Commands.CreateCar;

public class CreateCarCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) 
    : IRequestHandler<CreateCarCommand, int>
{
    public async Task<int> Handle(CreateCarCommand request, CancellationToken cancellationToken)
    {
        var car = mapper.Map<Car>(request);
        await unitOfWork.Repository<Car>().AddAsync(car);
        await unitOfWork.CompleteAsync();

        return car.Id;
    }
}
