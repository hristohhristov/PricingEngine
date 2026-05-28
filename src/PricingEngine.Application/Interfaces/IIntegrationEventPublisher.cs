using PricingEngine.Domain.Common;

namespace PricingEngine.Application.Interfaces;

public interface IIntegrationEventPublisher
{
    Task Publish<T>(T @event, CancellationToken ct = default)
        where T : class, IIntegrationEvent;
}
