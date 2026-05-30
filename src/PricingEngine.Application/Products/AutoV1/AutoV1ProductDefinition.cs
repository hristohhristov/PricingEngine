using System.Text.Json;
using PricingEngine.Application.Products.Common;
using PricingEngine.Domain.Pricing.Models;

namespace PricingEngine.Application.Products.AutoV1;

/// <summary>
/// Pricing strategy and product definition for <c>AUTO_V1</c> (automobile insurance, version 1).
/// Net premium = vehicleValue × BaseRate × AgeTierMultiplier × CoverageMultiplier [+ ComprehensiveRiderFee]; fee = AdminFee (flat).
/// </summary>
public sealed class AutoV1ProductDefinition : ProductDefinitionBase<AutoV1ProductDefinition.Config>
{
    /// <inheritdoc/>
    public override string SupportedProductCode => "AUTO_V1";

    /// <inheritdoc/>
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

    /// <summary>
    /// Calculates the net premium and admin fee for an automobile insurance quote.
    /// Applies driver-age and coverage-type multipliers and optionally adds a comprehensive rider fee.
    /// </summary>
    /// <param name="parameters">
    /// JSON payload; must contain <c>vehicleValue</c> (decimal), <c>driverAge</c> (int),
    /// <c>coverageType</c> (string), and optionally <c>comprehensiveRider</c> (bool).
    /// </param>
    /// <param name="config">The active product configuration with tariff tables.</param>
    /// <returns>A tuple of net premium and admin fee <see cref="Money"/> values.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <c>driverAge</c> falls outside all configured age tiers, or <c>coverageType</c> is not recognised.
    /// </exception>
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

    /// <summary>Strongly-typed configuration record for <c>AUTO_V1</c>.</summary>
    /// <param name="BaseRate">Percentage of vehicle value used as the base premium rate.</param>
    /// <param name="AdminFee">Flat administration fee added to every quote.</param>
    /// <param name="ComprehensiveRiderFee">Additional fee charged when the optional comprehensive rider is selected.</param>
    /// <param name="AgeTiers">Ordered list of age-bracket multipliers applied to the base premium.</param>
    /// <param name="CoverageMultipliers">Map from coverage type name to its premium multiplier.</param>
    public sealed record Config(
        decimal BaseRate,
        decimal AdminFee,
        decimal ComprehensiveRiderFee,
        List<AgeTier> AgeTiers,
        Dictionary<string, decimal> CoverageMultipliers);

    /// <summary>Defines a driver age bracket and the multiplier applied to the base premium.</summary>
    /// <param name="MinAge">Minimum driver age (inclusive) for this tier.</param>
    /// <param name="MaxAge">Maximum driver age (inclusive) for this tier.</param>
    /// <param name="Multiplier">Factor applied to the base rate for drivers in this age range.</param>
    public sealed record AgeTier(int MinAge, int MaxAge, decimal Multiplier);
}
