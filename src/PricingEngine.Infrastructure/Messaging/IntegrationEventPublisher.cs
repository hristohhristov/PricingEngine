using MassTransit;
using PricingEngine.Application.Interfaces;
using PricingEngine.Domain.Common;

namespace PricingEngine.Infrastructure.Messaging;

/// <summary>
/// MassTransit-backed implementation of <see cref="IIntegrationEventPublisher"/>.
/// Delegates event publishing to the <see cref="IPublishEndpoint"/> provided by MassTransit.
/// </summary>
public class IntegrationEventPublisher(IPublishEndpoint publishEndpoint)
    : IIntegrationEventPublisher
{
    /// <summary>
    /// Publishes the specified integration event through the MassTransit publish endpoint.
    /// </summary>
    /// <typeparam name="T">The integration event type; must implement <see cref="IIntegrationEvent"/>.</typeparam>
    /// <param name="event">The event payload to publish.</param>
    /// <param name="ct">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the event has been accepted by the broker.</returns>
    public Task Publish<T>(T @event, CancellationToken ct = default)
        where T : class, IIntegrationEvent
        => publishEndpoint.Publish(@event, ct);
}
