using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Core.Interfaces;
using MediatR;

namespace Agentic_Rentify.Application.Features.Attractions.Commands.DeleteAttraction;

public class DeleteAttractionCommandHandler : IRequestHandler<DeleteAttractionCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAttractionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteAttractionCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Attraction>();
        var attraction = await repository.GetByIdAsync(request.Id);

        if (attraction == null)
        {
            throw new Exception($"Attraction with ID {request.Id} not found.");
        }

        // Hard Delete or Soft Delete based on requirements. 
        // BaseEntity usually has IsDeleted.
        attraction.IsDeleted = true;
        await repository.UpdateAsync(attraction);
        // OR await repository.DeleteAsync(attraction); if hard delete.
        // Given 'IsDeleted' property in BaseEntity/migration, Soft Delete is preferred.
        
        await _unitOfWork.CompleteAsync();
        return true;
    }
}
