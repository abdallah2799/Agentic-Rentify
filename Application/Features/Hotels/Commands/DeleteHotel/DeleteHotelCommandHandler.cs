using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Application.Features.SyncVector;
using MediatR;

namespace Agentic_Rentify.Application.Features.Hotels.Commands.DeleteHotel;

public class DeleteHotelCommandHandler(IUnitOfWork unitOfWork, IMediator mediator) 
    : IRequestHandler<DeleteHotelCommand, int>
{
    public async Task<int> Handle(DeleteHotelCommand request, CancellationToken cancellationToken)
    {
        var hotel = await unitOfWork.Repository<Hotel>().GetByIdAsync(request.Id);
        if (hotel == null)
        {
            throw new Exception($"Hotel with ID {request.Id} not found.");
        }

        await unitOfWork.Repository<Hotel>().DeleteAsync(hotel);
        await unitOfWork.CompleteAsync();

        await mediator.Publish(new EntityDeletedFromVectorDbEvent(hotel.Id, "Hotel"), cancellationToken);

        return hotel.Id;
    }
}
