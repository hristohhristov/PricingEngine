using System.Text.Json;
using PricingEngine.Domain.Pricing.Models;

namespace PricingEngine.Application.Strategies.HomeV1;

public sealed class SimpleHomePricingStrategy : BasePricingStrategy
{
    public override string SupportedProductCode => "HOME_V1";

    protected override (Money netPremium, Money fee) CalculateComponents(
        JsonElement parameters, string configurationJson)
    {
        var insuredSum = parameters.GetProperty("insuredSum").GetDecimal();
        var config = JsonSerializer.Deserialize<HomeV1Config>(configurationJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        var premium  = new Money(insuredSum).MultiplyBy(config.BaseTariff).Round();
        var fixedFee = new Money(config.FixedFee);
        return (premium, fixedFee);
    }

    // Seed JSON shape: { "BaseTariff": 0.005, "FixedFee": 25.00 }
    private sealed record HomeV1Config(decimal BaseTariff, decimal FixedFee);
}
