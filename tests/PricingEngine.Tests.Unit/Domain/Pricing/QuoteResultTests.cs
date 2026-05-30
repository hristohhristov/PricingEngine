using FluentAssertions;
using PricingEngine.Domain.Pricing.Models;

namespace PricingEngine.Tests.Unit.Domain.Pricing;

public class QuoteResultTests
{
    private static Money Eur(decimal amount) => new(amount, "EUR");

    [Fact]
    public void TotalAmount_IsNetPlusTaxPlusFee()
    {
        var result = new QuoteResult(Eur(500m), Eur(50m), Eur(25m));
        result.TotalAmount.Amount.Should().Be(575m);
    }

    [Fact]
    public void TotalAmount_WithZeroComponents_IsZero()
    {
        var result = new QuoteResult(Eur(0m), Eur(0m), Eur(0m));
        result.TotalAmount.Amount.Should().Be(0m);
    }

    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        var a = new QuoteResult(Eur(500m), Eur(50m), Eur(25m));
        var b = new QuoteResult(Eur(500m), Eur(50m), Eur(25m));
        a.Should().Be(b);
    }

    [Fact]
    public void Equality_DifferentNetPremium_NotEqual()
    {
        var a = new QuoteResult(Eur(500m), Eur(50m), Eur(25m));
        var b = new QuoteResult(Eur(400m), Eur(50m), Eur(25m));
        a.Should().NotBe(b);
    }

    [Fact]
    public void Equality_DifferentTax_NotEqual()
    {
        var a = new QuoteResult(Eur(500m), Eur(50m),  Eur(25m));
        var b = new QuoteResult(Eur(500m), Eur(100m), Eur(25m));
        a.Should().NotBe(b);
    }

    [Fact]
    public void Equality_DifferentFee_NotEqual()
    {
        var a = new QuoteResult(Eur(500m), Eur(50m), Eur(25m));
        var b = new QuoteResult(Eur(500m), Eur(50m), Eur(40m));
        a.Should().NotBe(b);
    }

    [Fact]
    public void Properties_ExposedCorrectly()
    {
        var net = Eur(500m);
        var tax = Eur(50m);
        var fee = Eur(25m);

        var result = new QuoteResult(net, tax, fee);

        result.NetPremium.Should().Be(net);
        result.TaxAmount.Should().Be(tax);
        result.FeeAmount.Should().Be(fee);
    }
}
