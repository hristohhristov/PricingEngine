using System.Text.Json;
using PricingEngine.Application.Products.Common;
using PricingEngine.Domain.Pricing.Models;

namespace PricingEngine.Application.Products.HomeV1;

/// <summary>
/// Pricing strategy and product definition for <c>HOME_V1</c> (home contents insurance, version 1).
/// Net premium = insuredSum × BaseTariff; tax (10%) is applied by the base class; fee is a flat FixedFee.
/// </summary>
public sealed class HomeV1ProductDefinition : ProductDefinitionBase<HomeV1ProductDefinition.Config>
{
    /// <inheritdoc/>
    public override string SupportedProductCode => "HOME_V1";

    /// <inheritdoc/>
    protected override Config DefaultConfig => new(BaseTariff: 0.005m, FixedFee: 25.00m);

    /// <summary>
    /// Calculates the net premium and fee for a home insurance quote.
    /// Net premium is derived by multiplying <c>insuredSum</c> from the payload by the configured base tariff.
    /// </summary>
    /// <param name="parameters">JSON payload; must contain a numeric <c>insuredSum</c> property.</param>
    /// <param name="config">The active product configuration.</param>
    /// <returns>A tuple of net premium and fixed fee <see cref="Money"/> values.</returns>
    protected override (Money netPremium, Money fee) Calculate(
        JsonElement parameters, Config config)
    {
        var insuredSum = parameters.GetProperty("insuredSum").GetDecimal();
        return (new Money(insuredSum).MultiplyBy(config.BaseTariff).Round(), new Money(config.FixedFee));
    }

    /// <summary>Strongly-typed configuration record for <c>HOME_V1</c>.</summary>
    /// <param name="BaseTariff">Rate applied to the insured sum to derive the net premium (e.g., 0.005 for 0.5%).</param>
    /// <param name="FixedFee">Flat administration fee added to every quote regardless of insured sum.</param>
    public sealed record Config(decimal BaseTariff, decimal FixedFee);
}
