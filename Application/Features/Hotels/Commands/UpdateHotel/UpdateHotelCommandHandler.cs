using Agentic_Rentify.Application.Features.Hotels.DTOs;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using AutoMapper;
using MediatR;
using Agentic_Rentify.Application.Features.SyncVector;

namespace Agentic_Rentify.Application.Features.Hotels.Commands.UpdateHotel;

public class UpdateHotelCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator) 
    : IRequestHandler<UpdateHotelCommand, int>
{
    public async Task<int> Handle(UpdateHotelCommand request, CancellationToken cancellationToken)
    {
        var hotel = await unitOfWork.Repository<Hotel>().GetByIdAsync(request.Id);
        if (hotel == null)
        {
            throw new Exception($"Hotel with ID {request.Id} not found.");
        }

        mapper.Map(request, hotel);
        await unitOfWork.Repository<Hotel>().UpdateAsync(hotel);
        await unitOfWork.CompleteAsync();

        var text = string.Join(" ", new[] { hotel.Name, hotel.Description });
        await mediator.Publish(new EntitySavedToVectorDbEvent(
            hotel.Id,
            "Hotel",
            text,
            name: hotel.Name,
            price: hotel.BasePrice,
            city: hotel.City), cancellationToken);

        return hotel.Id;
    }
}
