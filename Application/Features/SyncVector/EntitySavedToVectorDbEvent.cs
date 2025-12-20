using Agentic_Rentify.Core.Common;
using MediatR;

namespace Agentic_Rentify.Application.Features.SyncVector;

public sealed class EntitySavedToVectorDbEvent(int entityId, string entityType, string text)
    : BaseDomainEvent, INotification
{
    public int EntityId { get; } = entityId;
    public string EntityType { get; } = entityType;
    public string Text { get; } = text;
}
