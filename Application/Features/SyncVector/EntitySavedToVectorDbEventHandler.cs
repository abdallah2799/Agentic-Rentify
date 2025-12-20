using Agentic_Rentify.Application.Interfaces;
using MediatR;

namespace Agentic_Rentify.Application.Features.SyncVector;

public class EntitySavedToVectorDbEventHandler(IVectorDbService vectorDbService)
    : INotificationHandler<EntitySavedToVectorDbEvent>
{
    public async Task Handle(EntitySavedToVectorDbEvent notification, CancellationToken cancellationToken)
    {
        const string collection = "rentify_memory";
        await vectorDbService.CreateCollectionIfNotExists(collection);
        await vectorDbService.SaveTextVector(
            collection,
            notification.EntityId.ToString(),
            notification.EntityType,
            notification.Text,
            notification.Name,
            notification.Price,
            notification.City);
    }
}
