using MassTransit;
using PricingEngine.Application.Interfaces;
using PricingEngine.Domain.Common;

namespace PricingEngine.Infrastructure.Messaging;

public class IntegrationEventPublisher(IPublishEndpoint publishEndpoint)
    : IIntegrationEventPublisher
{
    public Task Publish<T>(T @event, CancellationToken ct = default)
        where T : class, IIntegrationEvent
        => publishEndpoint.Publish(@event, ct);
}
