using System.Text.Json;
using PricingEngine.Domain.Pricing.Interfaces;
using PricingEngine.Domain.Pricing.Models;

namespace PricingEngine.Application.Strategies;

public abstract class BasePricingStrategy : IProductPricingStrategy
{
    private const decimal TaxRate = 0.10m;

    public abstract string SupportedProductCode { get; }

    public QuoteResult Calculate(JsonElement parameters, string configurationJson)
    {
        var (netPremium, fee) = CalculateComponents(parameters, configurationJson);
        var tax = netPremium.Percentage(TaxRate).Round();
        return new QuoteResult(netPremium, tax, fee);
    }

    protected abstract (Money netPremium, Money fee) CalculateComponents(
        JsonElement parameters, string configurationJson);
}
