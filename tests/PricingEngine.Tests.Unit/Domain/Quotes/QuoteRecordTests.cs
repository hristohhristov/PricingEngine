using FluentAssertions;
using PricingEngine.Domain.Pricing.Models;
using PricingEngine.Domain.Quotes.Enums;
using PricingEngine.Domain.Quotes.Events;
using PricingEngine.Domain.Quotes.Exceptions;
using PricingEngine.Domain.Quotes.Factories;
using PricingEngine.Domain.Quotes.Models;

namespace PricingEngine.Tests.Unit.Domain.Quotes;

public class QuoteRecordTests
{
    private static QuoteRecord BuildValidQuote(
        string productCode = "HOME_V1",
        decimal net        = 500m,
        decimal tax        = 50m,
        decimal fee        = 25m)
    {
        var result = new QuoteResult(
            new Money(net, "EUR"),
            new Money(tax, "EUR"),
            new Money(fee, "EUR"));

        return new QuoteRecordFactory()
            .WithProductCode(productCode)
            .WithQuoteDate(DateTime.UtcNow)
            .WithQuoteResult(result)
            .Build();
    }

    // ── Status ───────────────────────────────────────────────────────────────

    [Fact]
    public void Build_ValidInputs_StatusIsPending()
    {
        var quote = BuildValidQuote();
        quote.Status.Should().Be(QuoteStatus.Pending);
    }

    // ── Domain events ─────────────────────────────────────────────────────────

    [Fact]
    public void Build_ValidInputs_RaisesQuoteGeneratedDomainEvent()
    {
        var quote = BuildValidQuote();
        quote.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<QuoteGeneratedDomainEvent>();
    }

    // ── Total amount ──────────────────────────────────────────────────────────

    [Fact]
    public void Build_ValidInputs_TotalAmountComputed()
    {
        var quote = BuildValidQuote(net: 500m, tax: 50m, fee: 25m);
        quote.TotalAmount.Should().Be(575m);
    }

    // ── MarkAsAudited ─────────────────────────────────────────────────────────

    [Fact]
    public void MarkAsAudited_WhenPending_ChangesStatusToAudited()
    {
        var quote = BuildValidQuote();
        quote.MarkAsAudited();
        quote.Status.Should().Be(QuoteStatus.Audited);
    }

    [Fact]
    public void MarkAsAudited_WhenAlreadyAudited_ThrowsInvalidQuoteException()
    {
        var quote = BuildValidQuote();
        quote.MarkAsAudited();

        var act = () => quote.MarkAsAudited();
        act.Should().Throw<InvalidQuoteException>()
            .WithMessage("*Pending*");
    }

    // ── Guard on product code ─────────────────────────────────────────────────

    [Fact]
    public void Build_EmptyProductCode_ThrowsInvalidQuoteException()
    {
        var result = new QuoteResult(new Money(500m), new Money(50m), new Money(25m));
        var act = () => new QuoteRecordFactory()
            .WithProductCode("")
            .WithQuoteDate(DateTime.UtcNow)
            .WithQuoteResult(result)
            .Build();

        act.Should().Throw<InvalidQuoteException>();
    }

    // ── Properties ───────────────────────────────────────────────────────────

    [Fact]
    public void Build_ValidInputs_ProductCodeStoredCorrectly()
    {
        var quote = BuildValidQuote(productCode: "AUTO_V1");
        quote.ProductCode.Should().Be("AUTO_V1");
    }

    [Fact]
    public void Build_ValidInputs_CurrencyDefaultsToEur()
    {
        var quote = BuildValidQuote();
        quote.Currency.Should().Be("EUR");
    }

    [Fact]
    public void Build_ValidInputs_IdIsNotEmpty()
    {
        var quote = BuildValidQuote();
        quote.Id.Should().NotBeEmpty();
    }
}
