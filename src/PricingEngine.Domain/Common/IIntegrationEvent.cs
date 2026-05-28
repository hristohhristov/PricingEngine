namespace PricingEngine.Domain.Common;

public interface IIntegrationEvent : IDomainEvent
{
    Guid     EventId    { get; }
    string   EventType  { get; }
    string   Version    { get; }
    DateTime OccurredAt { get; }
}
