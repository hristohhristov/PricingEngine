using System.Text.Json;
using FluentAssertions;
using PricingEngine.Application.Products.HomeV1;

namespace PricingEngine.Tests.Unit.Application.Products;

public class HomeV1ProductDefinitionTests
{
    private readonly HomeV1ProductDefinition _definition = new();

    /// <summary>Build default config JSON from the definition itself (tests deserialization too).</summary>
    private string DefaultConfigJson => _definition.DefaultConfigJson;

    private static JsonElement Payload(decimal insuredSum)
        => JsonDocument.Parse($$"""{"insuredSum":{{insuredSum}}}""").RootElement;

    // ── Product code ─────────────────────────────────────────────────────────

    [Fact]
    public void SupportedProductCode_IsHomeV1()
    {
        _definition.SupportedProductCode.Should().Be("HOME_V1");
    }

    // ── Calculation ───────────────────────────────────────────────────────────

    [Fact]
    public void Calculate_InsuredSum100000_NetPremium500()
    {
        // 100_000 × 0.005 = 500
        var result = _definition.Calculate(Payload(100_000m), DefaultConfigJson);
        result.NetPremium.Amount.Should().Be(500m);
    }

    [Fact]
    public void Calculate_Fee_Is25()
    {
        var result = _definition.Calculate(Payload(100_000m), DefaultConfigJson);
        result.FeeAmount.Amount.Should().Be(25m);
    }

    [Fact]
    public void Calculate_Tax_Is10PercentOfNetPremium()
    {
        // net = 500, tax = 10% × 500 = 50
        var result = _definition.Calculate(Payload(100_000m), DefaultConfigJson);
        result.TaxAmount.Amount.Should().Be(50m);
    }

    [Fact]
    public void Calculate_TotalAmount_IsNetPlusTaxPlusFee()
    {
        // 500 + 50 + 25 = 575
        var result = _definition.Calculate(Payload(100_000m), DefaultConfigJson);
        result.TotalAmount.Amount.Should().Be(575m);
    }

    [Fact]
    public void Calculate_SmallInsuredSum_RoundsCorrectly()
    {
        // 1000 × 0.005 = 5.00, tax = 0.50, fee = 25, total = 30.50
        var result = _definition.Calculate(Payload(1_000m), DefaultConfigJson);
        result.NetPremium.Amount.Should().Be(5m);
        result.TaxAmount.Amount.Should().Be(0.50m);
        result.TotalAmount.Amount.Should().Be(30.50m);
    }

    // ── Default config JSON ───────────────────────────────────────────────────

    [Fact]
    public void DefaultConfigJson_IsValidJson()
    {
        var act = () => JsonDocument.Parse(DefaultConfigJson);
        act.Should().NotThrow();
    }

    [Fact]
    public void DefaultConfigJson_ContainsBaseTariff()
    {
        var doc = JsonDocument.Parse(DefaultConfigJson);
        // The serializer preserves the C# property name casing; try both conventions
        var found = doc.RootElement.TryGetProperty("BaseTariff", out _)
                 || doc.RootElement.TryGetProperty("baseTariff", out _);
        found.Should().BeTrue("DefaultConfigJson must contain a BaseTariff (or baseTariff) field");
    }

    [Fact]
    public void DefaultConfigJson_ContainsFixedFee()
    {
        var doc = JsonDocument.Parse(DefaultConfigJson);
        var found = doc.RootElement.TryGetProperty("FixedFee", out _)
                 || doc.RootElement.TryGetProperty("fixedFee", out _);
        found.Should().BeTrue("DefaultConfigJson must contain a FixedFee (or fixedFee) field");
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
