using System.Text.Json;
using PricingEngine.Domain.Pricing.Interfaces;
using PricingEngine.Domain.Pricing.Models;

namespace PricingEngine.Application.Strategies;

/// <summary>
/// Abstract base class for all product pricing strategies.
/// Applies a fixed 10% tax on top of the net premium computed by concrete subclasses.
/// </summary>
public abstract class BasePricingStrategy : IProductPricingStrategy
{
    private const decimal TaxRate = 0.10m;

    /// <inheritdoc/>
    public abstract string SupportedProductCode { get; }

    /// <summary>
    /// Computes the full <see cref="QuoteResult"/> by delegating component calculation to the subclass
    /// and appending the 10% tax on the net premium.
    /// </summary>
    /// <param name="parameters">The product-specific JSON payload from the API request.</param>
    /// <param name="configurationJson">The product's active configuration serialised as JSON.</param>
    /// <returns>A <see cref="QuoteResult"/> containing net premium, tax (10%), and fee.</returns>
    public QuoteResult Calculate(JsonElement parameters, string configurationJson)
    {
        var (netPremium, fee) = CalculateComponents(parameters, configurationJson);
        var tax = netPremium.Percentage(TaxRate).Round();
        return new QuoteResult(netPremium, tax, fee);
    }

    /// <summary>
    /// Template method implemented by concrete strategies to compute the net premium and flat fee.
    /// The base class is responsible for tax calculation.
    /// </summary>
    /// <param name="parameters">The product-specific JSON payload from the API request.</param>
    /// <param name="configurationJson">The product's active configuration serialised as JSON.</param>
    /// <returns>A tuple of net premium and fee <see cref="Money"/> values.</returns>
    protected abstract (Money netPremium, Money fee) CalculateComponents(
        JsonElement parameters, string configurationJson);
}
