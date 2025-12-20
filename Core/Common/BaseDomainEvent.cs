namespace Agentic_Rentify.Core.Common;

public abstract class BaseDomainEvent : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
