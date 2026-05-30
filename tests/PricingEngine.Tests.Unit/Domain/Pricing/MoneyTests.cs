using FluentAssertions;
using PricingEngine.Domain.Pricing.Exceptions;
using PricingEngine.Domain.Pricing.Models;

namespace PricingEngine.Tests.Unit.Domain.Pricing;

public class MoneyTests
{
    // ── Construction ────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_ValidAmount_CreatesInstance()
    {
        var money = new Money(500m, "EUR");
        money.Amount.Should().Be(500m);
        money.Currency.Should().Be("EUR");
    }

    [Fact]
    public void Constructor_ZeroAmount_CreatesInstance()
    {
        var money = new Money(0m, "EUR");
        money.Amount.Should().Be(0m);
    }

    [Fact]
    public void Constructor_NegativeAmount_Throws()
    {
        var act = () => new Money(-1m, "EUR");
        act.Should().Throw<InvalidMoneyException>();
    }

    [Fact]
    public void Constructor_EmptyCurrency_Throws()
    {
        var act = () => new Money(100m, "");
        act.Should().Throw<InvalidMoneyException>();
    }

    [Fact]
    public void Constructor_WhiteSpaceCurrency_Throws()
    {
        var act = () => new Money(100m, "   ");
        act.Should().Throw<InvalidMoneyException>();
    }

    // ── Static factories ────────────────────────────────────────────────────

    [Fact]
    public void Zero_ReturnsMoneyWithZeroAmount()
    {
        var zero = Money.Zero("EUR");
        zero.Amount.Should().Be(0m);
        zero.Currency.Should().Be("EUR");
    }

    [Fact]
    public void Zero_DefaultCurrencyIsEur()
    {
        Money.Zero().Currency.Should().Be("EUR");
    }

    // ── Add ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Add_SameCurrency_ReturnsSum()
    {
        var a = new Money(100m, "EUR");
        var b = new Money(50m,  "EUR");
        a.Add(b).Amount.Should().Be(150m);
    }

    [Fact]
    public void Add_DifferentCurrency_Throws()
    {
        var eur = new Money(100m, "EUR");
        var usd = new Money(100m, "USD");
        var act = () => eur.Add(usd);
        act.Should().Throw<InvalidMoneyException>();
    }

    // ── Subtract ────────────────────────────────────────────────────────────

    [Fact]
    public void Subtract_SameCurrency_ValidResult()
    {
        var a = new Money(100m, "EUR");
        var b = new Money(40m,  "EUR");
        a.Subtract(b).Amount.Should().Be(60m);
    }

    [Fact]
    public void Subtract_WouldGoNegative_Throws()
    {
        var a = new Money(10m, "EUR");
        var b = new Money(50m, "EUR");
        var act = () => a.Subtract(b);
        act.Should().Throw<InvalidMoneyException>();
    }

    [Fact]
    public void Subtract_DifferentCurrency_Throws()
    {
        var eur = new Money(100m, "EUR");
        var usd = new Money(50m,  "USD");
        var act = () => eur.Subtract(usd);
        act.Should().Throw<InvalidMoneyException>();
    }

    // ── MultiplyBy ──────────────────────────────────────────────────────────

    [Fact]
    public void MultiplyBy_ValidFactor_ReturnsProduct()
    {
        var money = new Money(100m, "EUR");
        money.MultiplyBy(1.5m).Amount.Should().Be(150m);
    }

    [Fact]
    public void MultiplyBy_ZeroFactor_ReturnsZero()
    {
        var money = new Money(100m, "EUR");
        money.MultiplyBy(0m).Amount.Should().Be(0m);
    }

    [Fact]
    public void MultiplyBy_NegativeFactor_Throws()
    {
        var money = new Money(100m, "EUR");
        var act = () => money.MultiplyBy(-1m);
        act.Should().Throw<InvalidMoneyException>();
    }

    // ── Percentage ──────────────────────────────────────────────────────────

    [Fact]
    public void Percentage_ValidRate_ReturnsPercentage()
    {
        var money = new Money(200m, "EUR");
        money.Percentage(0.10m).Amount.Should().Be(20m);
    }

    [Fact]
    public void Percentage_ZeroRate_ReturnsZero()
    {
        var money = new Money(200m, "EUR");
        money.Percentage(0m).Amount.Should().Be(0m);
    }

    [Fact]
    public void Percentage_RateAboveOne_Throws()
    {
        var money = new Money(100m, "EUR");
        var act = () => money.Percentage(1.1m);
        act.Should().Throw<InvalidMoneyException>();
    }

    [Fact]
    public void Percentage_RateBelowZero_Throws()
    {
        var money = new Money(100m, "EUR");
        var act = () => money.Percentage(-0.1m);
        act.Should().Throw<InvalidMoneyException>();
    }

    [Fact]
    public void Percentage_RateEqualToOne_ReturnsFullAmount()
    {
        var money = new Money(100m, "EUR");
        money.Percentage(1m).Amount.Should().Be(100m);
    }

    // ── Round ───────────────────────────────────────────────────────────────

    [Fact]
    public void Round_RoundsToTwoDecimals_AwayFromZero()
    {
        var money = new Money(100.555m, "EUR");
        money.Round().Amount.Should().Be(100.56m);
    }

    [Fact]
    public void Round_AlreadyTwoDecimals_NoChange()
    {
        var money = new Money(100.50m, "EUR");
        money.Round().Amount.Should().Be(100.50m);
    }

    // ── Equality (ValueObject) ───────────────────────────────────────────────

    [Fact]
    public void Equality_SameAmountAndCurrency_AreEqual()
    {
        var a = new Money(100m, "EUR");
        var b = new Money(100m, "EUR");
        a.Should().Be(b);
    }

    [Fact]
    public void Equality_DifferentAmount_NotEqual()
    {
        var a = new Money(100m, "EUR");
        var b = new Money(200m, "EUR");
        a.Should().NotBe(b);
    }

    [Fact]
    public void Equality_DifferentCurrency_NotEqual()
    {
        var a = new Money(100m, "EUR");
        var b = new Money(100m, "USD");
        a.Should().NotBe(b);
    }
}
