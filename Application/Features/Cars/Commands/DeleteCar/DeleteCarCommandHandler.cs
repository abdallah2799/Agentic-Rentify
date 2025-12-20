using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Application.Features.SyncVector;
using MediatR;

namespace Agentic_Rentify.Application.Features.Cars.Commands.DeleteCar;

public class DeleteCarCommandHandler(IUnitOfWork unitOfWork, IMediator mediator) 
    : IRequestHandler<DeleteCarCommand, int>
{
    public async Task<int> Handle(DeleteCarCommand request, CancellationToken cancellationToken)
    {
        var car = await unitOfWork.Repository<Car>().GetByIdAsync(request.Id);
        if (car == null)
        {
            throw new Exception($"Car with ID {request.Id} not found.");
        }

        await unitOfWork.Repository<Car>().DeleteAsync(car);
        await unitOfWork.CompleteAsync();

        await mediator.Publish(new EntityDeletedFromVectorDbEvent(car.Id, "Car"), cancellationToken);

        return car.Id;
    }
}
