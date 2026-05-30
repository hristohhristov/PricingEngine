using System.Text.Json;
using PricingEngine.Domain.Pricing.Models;

namespace PricingEngine.Domain.Pricing.Interfaces;

/// <summary>
/// Strategy contract for computing a premium quote for a specific product.
/// Each implementation handles the calculation logic for exactly one product code.
/// </summary>
public interface IProductPricingStrategy
{
    /// <summary>Gets the product code this strategy handles (e.g., <c>"HOME_V1"</c>).</summary>
    string SupportedProductCode { get; }

    /// <summary>
    /// Calculates the net premium, tax, and fee for the given input parameters and product configuration.
    /// </summary>
    /// <param name="parameters">The JSON payload from the API request (product-specific fields).</param>
    /// <param name="configurationJson">The product configuration serialised as JSON.</param>
    /// <returns>A <see cref="QuoteResult"/> containing net premium, tax amount, and fee amount.</returns>
    QuoteResult Calculate(JsonElement parameters, string configurationJson);
}
