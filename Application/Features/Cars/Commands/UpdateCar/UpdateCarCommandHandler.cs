using Agentic_Rentify.Application.Features.Cars.DTOs;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using AutoMapper;
using MediatR;

namespace Agentic_Rentify.Application.Features.Cars.Commands.UpdateCar;

public class UpdateCarCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) 
    : IRequestHandler<UpdateCarCommand, int>
{
    public async Task<int> Handle(UpdateCarCommand request, CancellationToken cancellationToken)
    {
        var car = await unitOfWork.Repository<Car>().GetByIdAsync(request.Id);
        if (car == null)
        {
            throw new Exception($"Car with ID {request.Id} not found.");
        }

        mapper.Map(request, car);
        await unitOfWork.Repository<Car>().UpdateAsync(car);
        await unitOfWork.CompleteAsync();

        return car.Id;
    }
}
