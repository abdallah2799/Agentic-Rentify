using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Application.Features.SyncVector;
using MediatR;

namespace Agentic_Rentify.Application.Features.Attractions.Commands.DeleteAttraction;

public class DeleteAttractionCommandHandler : IRequestHandler<DeleteAttractionCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public DeleteAttractionCommandHandler(IUnitOfWork unitOfWork, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<bool> Handle(DeleteAttractionCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Attraction>();
        var attraction = await repository.GetByIdAsync(request.Id);

        if (attraction == null)
        {
            throw new Exception($"Attraction with ID {request.Id} not found.");
        }

        // Soft Delete
        attraction.IsDeleted = true;
        await repository.UpdateAsync(attraction);
        
        await _unitOfWork.CompleteAsync();

        await _mediator.Publish(new EntityDeletedFromVectorDbEvent(attraction.Id, "Attraction"), cancellationToken);

        return true;
    }
}
