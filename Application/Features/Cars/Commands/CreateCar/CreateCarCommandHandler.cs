using Agentic_Rentify.Application.Features.Cars.DTOs;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using AutoMapper;
using MediatR;
using Agentic_Rentify.Application.Features.SyncVector;

namespace Agentic_Rentify.Application.Features.Cars.Commands.CreateCar;

public class CreateCarCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator) 
    : IRequestHandler<CreateCarCommand, int>
{
    public async Task<int> Handle(CreateCarCommand request, CancellationToken cancellationToken)
    {
        var car = mapper.Map<Car>(request);
        await unitOfWork.Repository<Car>().AddAsync(car);
        await unitOfWork.CompleteAsync();

        var text = string.Join(" ", new[] { car.Name, car.Description, car.Overview });
        await mediator.Publish(new EntitySavedToVectorDbEvent(
            car.Id,
            "Car",
            text,
            name: car.Name,
            price: car.Price,
            city: null), cancellationToken);

        return car.Id;
    }
}
