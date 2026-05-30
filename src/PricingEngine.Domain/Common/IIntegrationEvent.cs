namespace PricingEngine.Domain.Common;

/// <summary>
/// Contract for integration events that cross bounded-context boundaries (e.g., via a message broker).
/// Extends <see cref="IDomainEvent"/> with envelope metadata required for routing and idempotency.
/// </summary>
public interface IIntegrationEvent : IDomainEvent
{
    /// <summary>Gets the unique identifier of this event instance; used for idempotency checks.</summary>
    Guid EventId { get; }

    /// <summary>Gets the logical name of the event type (e.g., <c>"QuoteGenerated"</c>).</summary>
    string EventType { get; }

    /// <summary>Gets the schema version of the event payload (e.g., <c>"v1"</c>).</summary>
    string Version { get; }

    /// <summary>Gets the UTC timestamp at which the event was raised.</summary>
    DateTime OccurredAt { get; }
}
