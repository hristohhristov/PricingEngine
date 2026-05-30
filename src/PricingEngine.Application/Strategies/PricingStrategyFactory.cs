using PricingEngine.Application.Exceptions;
using PricingEngine.Domain.Pricing.Interfaces;

namespace PricingEngine.Application.Strategies;

/// <summary>
/// Resolves the appropriate <see cref="IProductPricingStrategy"/> for a given product code.
/// Strategies are indexed case-insensitively at construction time from the injected collection.
/// </summary>
public sealed class PricingStrategyFactory(IEnumerable<IProductPricingStrategy> strategies)
{
    private readonly Dictionary<string, IProductPricingStrategy> _map =
        strategies.ToDictionary(s => s.SupportedProductCode, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Returns the pricing strategy registered for the given product code.
    /// </summary>
    /// <param name="productCode">The product identifier to look up (case-insensitive).</param>
    /// <returns>The <see cref="IProductPricingStrategy"/> that handles the product.</returns>
    /// <exception cref="UnsupportedProductException">Thrown when no strategy is registered for <paramref name="productCode"/>.</exception>
    public IProductPricingStrategy Resolve(string productCode)
    {
        if (_map.TryGetValue(productCode, out var strategy))
            return strategy;
        throw new UnsupportedProductException(productCode);
    }
}
