using PricingEngine.Domain.Common;

namespace PricingEngine.Application.Interfaces;

/// <summary>
/// Abstraction for publishing integration events to an external message broker.
/// Decouples the application layer from MassTransit and RabbitMQ infrastructure concerns.
/// </summary>
public interface IIntegrationEventPublisher
{
    /// <summary>
    /// Publishes the specified integration event to the configured message broker.
    /// </summary>
    /// <typeparam name="T">The integration event type; must implement <see cref="IIntegrationEvent"/>.</typeparam>
    /// <param name="event">The event payload to publish.</param>
    /// <param name="ct">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the event has been accepted by the broker.</returns>
    Task Publish<T>(T @event, CancellationToken ct = default)
        where T : class, IIntegrationEvent;
}
