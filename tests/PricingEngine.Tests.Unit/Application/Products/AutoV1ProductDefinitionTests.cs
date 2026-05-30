using System.Text.Json;
using FluentAssertions;
using PricingEngine.Application.Products.AutoV1;

namespace PricingEngine.Tests.Unit.Application.Products;

public class AutoV1ProductDefinitionTests
{
    private readonly AutoV1ProductDefinition _definition = new();
    private string DefaultConfigJson => _definition.DefaultConfigJson;

    private static JsonElement Payload(
        decimal vehicleValue,
        int     driverAge,
        string  coverageType,
        bool?   comprehensiveRider = null)
    {
        var riderPart = comprehensiveRider.HasValue
            ? $""","comprehensiveRider":{comprehensiveRider.Value.ToString().ToLower()}"""
            : "";

        return JsonDocument
            .Parse($$"""{"vehicleValue":{{vehicleValue}},"driverAge":{{driverAge}},"coverageType":"{{coverageType}}"{{riderPart}}}""")
            .RootElement;
    }

    // ── Product code ─────────────────────────────────────────────────────────

    [Fact]
    public void SupportedProductCode_IsAutoV1()
    {
        _definition.SupportedProductCode.Should().Be("AUTO_V1");
    }

    // ── Age tier: 26-45 (tier = 1.00), Third Party (cov = 1.00) ─────────────

    [Fact]
    public void Calculate_Age30_ThirdParty_NoRider()
    {
        // 10_000 × 0.03 × 1.00 × 1.00 = 300
        var result = _definition.Calculate(Payload(10_000m, 30, "Third Party"), DefaultConfigJson);
        result.NetPremium.Amount.Should().Be(300m);
        result.FeeAmount.Amount.Should().Be(40m);
    }

    // ── Age tier: 18-25 (tier = 1.60), Comprehensive (cov = 1.75) ────────────

    [Fact]
    public void Calculate_Age20_Comprehensive_NoRider()
    {
        // 10_000 × 0.03 × 1.60 × 1.75 = 840
        var result = _definition.Calculate(Payload(10_000m, 20, "Comprehensive"), DefaultConfigJson);
        result.NetPremium.Amount.Should().Be(840m);
    }

    // ── Age tier: 46-65 (tier = 1.10), Third Party Fire (cov = 1.25) ─────────

    [Fact]
    public void Calculate_Age55_ThirdPartyFire_NoRider()
    {
        // 10_000 × 0.03 × 1.10 × 1.25 = 412.50
        var result = _definition.Calculate(Payload(10_000m, 55, "Third Party Fire"), DefaultConfigJson);
        result.NetPremium.Amount.Should().Be(412.50m);
    }

    // ── Age tier: 66-99 (tier = 1.35) ────────────────────────────────────────

    [Fact]
    public void Calculate_Age70_ThirdParty_NoRider()
    {
        // 10_000 × 0.03 × 1.35 × 1.00 = 405
        var result = _definition.Calculate(Payload(10_000m, 70, "Third Party"), DefaultConfigJson);
        result.NetPremium.Amount.Should().Be(405m);
    }

    // ── Comprehensive rider ───────────────────────────────────────────────────

    [Fact]
    public void Calculate_WithComprehensiveRider_AddsRiderFee()
    {
        // base: 10_000 × 0.03 × 1.00 × 1.00 = 300; rider = 75; total net = 375
        var withRider    = _definition.Calculate(Payload(10_000m, 30, "Third Party", true),  DefaultConfigJson);
        var withoutRider = _definition.Calculate(Payload(10_000m, 30, "Third Party", false), DefaultConfigJson);

        (withRider.NetPremium.Amount - withoutRider.NetPremium.Amount).Should().Be(75m);
    }

    // ── Admin fee ────────────────────────────────────────────────────────────

    [Fact]
    public void Calculate_AdminFee_Is40()
    {
        var result = _definition.Calculate(Payload(10_000m, 30, "Third Party"), DefaultConfigJson);
        result.FeeAmount.Amount.Should().Be(40m);
    }

    // ── Error cases ───────────────────────────────────────────────────────────

    [Fact]
    public void Calculate_UnknownCoverageType_ThrowsInvalidOperationException()
    {
        var act = () => _definition.Calculate(
            Payload(10_000m, 30, "Unknown Coverage"), DefaultConfigJson);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Unknown coverage type*");
    }

    [Fact]
    public void Calculate_AgeOutOfAllTiers_ThrowsInvalidOperationException()
    {
        // Age 10 falls outside all configured tiers (18-99)
        var act = () => _definition.Calculate(
            Payload(10_000m, 10, "Third Party"), DefaultConfigJson);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*age tier*");
    }

    // ── Default validity dates ────────────────────────────────────────────────

    [Fact]
    public void DefaultValidFrom_Is2024()
    {
        _definition.DefaultValidFrom.Year.Should().Be(2024);
    }

    [Fact]
    public void DefaultValidTo_Is2100()
    {
        _definition.DefaultValidTo.Year.Should().Be(2100);
    }
}
