using Agentic_Rentify.Application.Features.Hotels.DTOs;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using AutoMapper;
using MediatR;
using Agentic_Rentify.Application.Features.SyncVector;

namespace Agentic_Rentify.Application.Features.Hotels.Commands.CreateHotel;

public class CreateHotelCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator) 
    : IRequestHandler<CreateHotelCommand, int>
{
    public async Task<int> Handle(CreateHotelCommand request, CancellationToken cancellationToken)
    {
        var hotel = mapper.Map<Hotel>(request);
        await unitOfWork.Repository<Hotel>().AddAsync(hotel);
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
