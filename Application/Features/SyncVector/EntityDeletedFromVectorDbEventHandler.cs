using Agentic_Rentify.Application.Interfaces;
using MediatR;

namespace Agentic_Rentify.Application.Features.SyncVector;

public class EntityDeletedFromVectorDbEventHandler(IVectorDbService vectorDbService)
    : INotificationHandler<EntityDeletedFromVectorDbEvent>
{
    public async Task Handle(EntityDeletedFromVectorDbEvent notification, CancellationToken cancellationToken)
    {
        const string collection = "rentify_memory";
        await vectorDbService.DeletePointAsync(collection, notification.EntityId.ToString(), notification.EntityType);
    }
}
