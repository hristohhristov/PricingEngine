using System.Text.Json;
using PricingEngine.Application.Products.Common;
using PricingEngine.Domain.Pricing.Models;

namespace PricingEngine.Application.Products.AutoV1;

/// <summary>
/// Automobile insurance v1.
///
/// netPremium = vehicleValue × BaseRate × AgeTierMultiplier × CoverageMultiplier
///            + ComprehensiveRiderFee (if rider requested)
/// fee        = AdminFee (flat)
/// Tax (10%) applied automatically by BasePricingStrategy.
///
/// Payload:
///   vehicleValue        decimal  — market value of the insured vehicle
///   driverAge           int      — age of primary driver in years
///   coverageType        string   — "Third Party" | "Third Party Fire" | "Comprehensive"
///   comprehensiveRider  bool?    — optional extended glass/windshield cover add-on
/// </summary>
public sealed class AutoV1ProductDefinition : ProductDefinitionBase<AutoV1ProductDefinition.Config>
{
    public override string SupportedProductCode => "AUTO_V1";

    protected override Config DefaultConfig => new(
        BaseRate: 0.03m,
        AdminFee: 40.00m,
        ComprehensiveRiderFee: 75.00m,
        AgeTiers:
        [
            new(MinAge: 18, MaxAge: 25, Multiplier: 1.60m),
            new(MinAge: 26, MaxAge: 45, Multiplier: 1.00m),
            new(MinAge: 46, MaxAge: 65, Multiplier: 1.10m),
            new(MinAge: 66, MaxAge: 99, Multiplier: 1.35m),
        ],
        CoverageMultipliers: new Dictionary<string, decimal>
        {
            ["Third Party"]      = 1.00m,
            ["Third Party Fire"] = 1.25m,
            ["Comprehensive"]    = 1.75m,
        }
    );

    protected override (Money netPremium, Money fee) Calculate(
        JsonElement parameters, Config config)
    {
        var vehicleValue = parameters.GetProperty("vehicleValue").GetDecimal();
        var driverAge    = parameters.GetProperty("driverAge").GetInt32();
        var coverage     = parameters.GetProperty("coverageType").GetString()!;
        var rider        = parameters.TryGetProperty("comprehensiveRider", out var p) && p.GetBoolean();

        var tier = config.AgeTiers.FirstOrDefault(t => driverAge >= t.MinAge && driverAge <= t.MaxAge)
            ?? throw new InvalidOperationException(
                $"No age tier configured for driver age {driverAge} in AUTO_V1.");

        if (!config.CoverageMultipliers.TryGetValue(coverage, out var coverageFactor))
            throw new InvalidOperationException(
                $"Unknown coverage type '{coverage}' for AUTO_V1.");

        var premium = new Money(vehicleValue)
            .MultiplyBy(config.BaseRate)
            .MultiplyBy(tier.Multiplier)
            .MultiplyBy(coverageFactor)
            .Add(rider ? new Money(config.ComprehensiveRiderFee) : Money.Zero())
            .Round();

        return (premium, new Money(config.AdminFee));
    }

    public sealed record Config(
        decimal BaseRate,
        decimal AdminFee,
        decimal ComprehensiveRiderFee,
        List<AgeTier> AgeTiers,
        Dictionary<string, decimal> CoverageMultipliers);

    public sealed record AgeTier(int MinAge, int MaxAge, decimal Multiplier);
}
