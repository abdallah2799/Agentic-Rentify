using Agentic_Rentify.Core.Common;
using MediatR;

namespace Agentic_Rentify.Application.Features.SyncVector;

public sealed class EntitySavedToVectorDbEvent(
    int entityId,
    string entityType,
    string text,
    string? name = null,
    decimal? price = null,
    string? city = null)
    : BaseDomainEvent, INotification
{
    public int EntityId { get; } = entityId;
    public string EntityType { get; } = entityType;
    public string Text { get; } = text;
    public string? Name { get; } = name;
    public decimal? Price { get; } = price;
    public string? City { get; } = city;
}
