using FluentAssertions;
using PricingEngine.Domain.Pricing.Models;
using PricingEngine.Domain.Quotes.Exceptions;
using PricingEngine.Domain.Quotes.Factories;

namespace PricingEngine.Tests.Unit.Domain.Quotes;

public class QuoteRecordFactoryTests
{
    private static QuoteResult ValidResult() =>
        new(new Money(500m), new Money(50m), new Money(25m));

    // ── Guard: missing required fields ───────────────────────────────────────

    [Fact]
    public void Build_WithoutProductCode_Throws()
    {
        var act = () => new QuoteRecordFactory()
            .WithQuoteDate(DateTime.UtcNow)
            .WithQuoteResult(ValidResult())
            .Build();

        act.Should().Throw<InvalidQuoteException>();
    }

    [Fact]
    public void Build_WithoutQuoteDate_Throws()
    {
        var act = () => new QuoteRecordFactory()
            .WithProductCode("HOME_V1")
            .WithQuoteResult(ValidResult())
            .Build();

        act.Should().Throw<InvalidQuoteException>();
    }

    [Fact]
    public void Build_WithoutQuoteResult_Throws()
    {
        var act = () => new QuoteRecordFactory()
            .WithProductCode("HOME_V1")
            .WithQuoteDate(DateTime.UtcNow)
            .Build();

        act.Should().Throw<InvalidQuoteException>();
    }

    // ── Happy path ───────────────────────────────────────────────────────────

    [Fact]
    public void Build_AllRequiredSet_ReturnsQuoteRecord()
    {
        var quote = new QuoteRecordFactory()
            .WithProductCode("HOME_V1")
            .WithQuoteDate(DateTime.UtcNow)
            .WithQuoteResult(ValidResult())
            .Build();

        quote.Should().NotBeNull();
        quote.Id.Should().NotBeEmpty();
    }

    // ── Currency ─────────────────────────────────────────────────────────────

    [Fact]
    public void DefaultCurrency_IsEur()
    {
        var quote = new QuoteRecordFactory()
            .WithProductCode("HOME_V1")
            .WithQuoteDate(DateTime.UtcNow)
            .WithQuoteResult(ValidResult())
            .Build();

        quote.Currency.Should().Be("EUR");
    }

    [Fact]
    public void WithCurrency_CustomCurrency_Applied()
    {
        var quote = new QuoteRecordFactory()
            .WithProductCode("HOME_V1")
            .WithQuoteDate(DateTime.UtcNow)
            .WithQuoteResult(ValidResult())
            .WithCurrency("USD")
            .Build();

        quote.Currency.Should().Be("USD");
    }

    // ── Fluent API ───────────────────────────────────────────────────────────

    [Fact]
    public void WithProductCode_ReturnsSameInstance()
    {
        var factory = new QuoteRecordFactory();
        var returned = factory.WithProductCode("HOME_V1");
        returned.Should().BeSameAs(factory);
    }

    [Fact]
    public void WithQuoteDate_ReturnsSameInstance()
    {
        var factory = new QuoteRecordFactory();
        var returned = factory.WithQuoteDate(DateTime.UtcNow);
        returned.Should().BeSameAs(factory);
    }

    [Fact]
    public void WithQuoteResult_ReturnsSameInstance()
    {
        var factory = new QuoteRecordFactory();
        var returned = factory.WithQuoteResult(ValidResult());
        returned.Should().BeSameAs(factory);
    }

    [Fact]
    public void WithCurrency_ReturnsSameInstance()
    {
        var factory = new QuoteRecordFactory();
        var returned = factory.WithCurrency("USD");
        returned.Should().BeSameAs(factory);
    }

    // ── Each Build() creates a unique Id ─────────────────────────────────────

    [Fact]
    public void Build_CalledTwice_ProducesDistinctIds()
    {
        var factory = new QuoteRecordFactory()
            .WithProductCode("HOME_V1")
            .WithQuoteDate(DateTime.UtcNow)
            .WithQuoteResult(ValidResult());

        // Each call to Build creates a new Guid
        var q1 = factory.Build();
        var q2 = factory.Build();

        q1.Id.Should().NotBe(q2.Id);
    }
}
