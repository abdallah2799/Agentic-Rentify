using Agentic_Rentify.Core.Common;
using MediatR;

namespace Agentic_Rentify.Application.Features.SyncVector;

public sealed class EntityDeletedFromVectorDbEvent(int entityId, string entityType)
    : BaseDomainEvent, INotification
{
    public int EntityId { get; } = entityId;
    public string EntityType { get; } = entityType;
}
