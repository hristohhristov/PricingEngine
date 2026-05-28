using PricingEngine.Application.Exceptions;
using PricingEngine.Domain.Pricing.Interfaces;

namespace PricingEngine.Application.Strategies;

public sealed class PricingStrategyFactory(IEnumerable<IProductPricingStrategy> strategies)
{
    private readonly Dictionary<string, IProductPricingStrategy> _map =
        strategies.ToDictionary(s => s.SupportedProductCode, StringComparer.OrdinalIgnoreCase);

    public IProductPricingStrategy Resolve(string productCode)
    {
        if (_map.TryGetValue(productCode, out var strategy))
            return strategy;
        throw new UnsupportedProductException(productCode);
    }
}
