using System.Text.Json;
using PricingEngine.Application.Products.Common;
using PricingEngine.Domain.Pricing.Models;

namespace PricingEngine.Application.Products.HomeV1;

public sealed class HomeV1ProductDefinition : ProductDefinitionBase<HomeV1ProductDefinition.Config>
{
    public override string SupportedProductCode => "HOME_V1";

    protected override Config DefaultConfig => new(BaseTariff: 0.005m, FixedFee: 25.00m);

    protected override (Money netPremium, Money fee) Calculate(
        JsonElement parameters, Config config)
    {
        var insuredSum = parameters.GetProperty("insuredSum").GetDecimal();
        return (new Money(insuredSum).MultiplyBy(config.BaseTariff).Round(), new Money(config.FixedFee));
    }

    public sealed record Config(decimal BaseTariff, decimal FixedFee);
}
