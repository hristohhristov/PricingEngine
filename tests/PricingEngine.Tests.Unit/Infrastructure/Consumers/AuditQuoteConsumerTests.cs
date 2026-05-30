using FluentAssertions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using PricingEngine.Domain.Pricing.Models;
using PricingEngine.Domain.Quotes.Enums;
using PricingEngine.Domain.Quotes.Events;
using PricingEngine.Domain.Quotes.Factories;
using PricingEngine.Domain.Quotes.Models;
using PricingEngine.Infrastructure.Messaging.Consumers;
using PricingEngine.Infrastructure.Persistence;

namespace PricingEngine.Tests.Unit.Infrastructure.Consumers;

public class AuditQuoteConsumerTests
{
    // ── Context factory ───────────────────────────────────────────────────────

    private static PricingDbContext BuildInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<PricingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new PricingDbContext(options);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static QuoteRecord BuildPendingQuote()
    {
        var result = new QuoteResult(
            new Money(500m, "EUR"),
            new Money(50m,  "EUR"),
            new Money(25m,  "EUR"));

        return new QuoteRecordFactory()
            .WithProductCode("HOME_V1")
            .WithQuoteDate(DateTime.UtcNow)
            .WithQuoteResult(result)
            .Build();
    }

    private static QuoteGeneratedIntegrationEvent EventFor(QuoteRecord quote)
        => new()
        {
            QuoteId     = quote.Id,
            ProductCode = quote.ProductCode,
            QuoteDate   = quote.QuoteDate,
            NetPremium  = quote.NetPremium,
            TaxAmount   = quote.TaxAmount,
            FeeAmount   = quote.FeeAmount,
            TotalAmount = quote.TotalAmount,
            Currency    = quote.Currency,
        };

    private static ConsumeContext<QuoteGeneratedIntegrationEvent> MockContext(
        QuoteGeneratedIntegrationEvent integrationEvent)
    {
        var ctx = Substitute.For<ConsumeContext<QuoteGeneratedIntegrationEvent>>();
        ctx.Message.Returns(integrationEvent);
        return ctx;
    }

    // ── Happy path ────────────────────────────────────────────────────────────

    [Fact]
    public async Task Consume_QuoteRecordExists_MarksAsAudited()
    {
        await using var context = BuildInMemoryContext();
        var quote = BuildPendingQuote();
        context.QuoteRecords.Add(quote);
        await context.SaveChangesAsync();

        var logger   = Substitute.For<ILogger<AuditQuoteConsumer>>();
        var consumer = new AuditQuoteConsumer(context, logger);
        var msgCtx   = MockContext(EventFor(quote));

        await consumer.Consume(msgCtx);

        var persisted = await context.QuoteRecords
            .IgnoreQueryFilters()
            .FirstAsync(q => q.Id == quote.Id);

        persisted.Status.Should().Be(QuoteStatus.Audited);
    }

    [Fact]
    public async Task Consume_QuoteRecordExists_CreatesAuditLog()
    {
        await using var context = BuildInMemoryContext();
        var quote = BuildPendingQuote();
        context.QuoteRecords.Add(quote);
        await context.SaveChangesAsync();

        var logger   = Substitute.For<ILogger<AuditQuoteConsumer>>();
        var consumer = new AuditQuoteConsumer(context, logger);
        var msgCtx   = MockContext(EventFor(quote));

        await consumer.Consume(msgCtx);

        var auditLog = await context.AuditLogs.FirstOrDefaultAsync(a => a.QuoteId == quote.Id);
        auditLog.Should().NotBeNull();
        auditLog!.ProductCode.Should().Be("HOME_V1");
    }

    [Fact]
    public async Task Consume_QuoteRecordExists_SavesChanges()
    {
        // Use a named database so a second context instance can read the same data
        const string dbName = "test_saves_changes";
        var options = new DbContextOptionsBuilder<PricingDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        await using var context = new PricingDbContext(options);
        var quote = BuildPendingQuote();
        context.QuoteRecords.Add(quote);
        await context.SaveChangesAsync();

        var logger   = Substitute.For<ILogger<AuditQuoteConsumer>>();
        var consumer = new AuditQuoteConsumer(context, logger);
        var msgCtx   = MockContext(EventFor(quote));

        await consumer.Consume(msgCtx);

        // Verify via a fresh context pointing at the same named in-memory database
        await using var readContext = new PricingDbContext(options);
        var auditCount = await readContext.AuditLogs.CountAsync();
        auditCount.Should().Be(1);
    }

    // ── Quote not found ───────────────────────────────────────────────────────

    [Fact]
    public async Task Consume_QuoteRecordNotFound_ThrowsInvalidOperationException()
    {
        await using var context = BuildInMemoryContext(); // empty — no records
        var logger   = Substitute.For<ILogger<AuditQuoteConsumer>>();
        var consumer = new AuditQuoteConsumer(context, logger);

        var missingEvent = new QuoteGeneratedIntegrationEvent
        {
            QuoteId     = Guid.NewGuid(),
            ProductCode = "HOME_V1",
            QuoteDate   = DateTime.UtcNow,
            NetPremium  = 500m,
            TaxAmount   = 50m,
            FeeAmount   = 25m,
            TotalAmount = 575m,
            Currency    = "EUR",
        };

        var msgCtx = MockContext(missingEvent);

        var act = async () => await consumer.Consume(msgCtx);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"*{missingEvent.QuoteId}*");
    }

    [Fact]
    public async Task Consume_QuoteRecordNotFound_LogsError()
    {
        await using var context = BuildInMemoryContext();
        var logger   = Substitute.For<ILogger<AuditQuoteConsumer>>();
        var consumer = new AuditQuoteConsumer(context, logger);

        var missingId = Guid.NewGuid();
        var missingEvent = new QuoteGeneratedIntegrationEvent
        {
            QuoteId = missingId,
            ProductCode = "HOME_V1",
            QuoteDate   = DateTime.UtcNow,
            Currency    = "EUR",
        };

        try { await consumer.Consume(MockContext(missingEvent)); }
        catch (InvalidOperationException) { /* expected */ }

        logger.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Is<object>(v => v.ToString()!.Contains(missingId.ToString())),
            Arg.Any<Exception?>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public async Task Consume_QuoteRecordNotFound_NoAuditLogCreated()
    {
        await using var context = BuildInMemoryContext();
        var logger   = Substitute.For<ILogger<AuditQuoteConsumer>>();
        var consumer = new AuditQuoteConsumer(context, logger);

        var missingEvent = new QuoteGeneratedIntegrationEvent
        {
            QuoteId     = Guid.NewGuid(),
            ProductCode = "HOME_V1",
            QuoteDate   = DateTime.UtcNow,
            Currency    = "EUR",
        };

        try { await consumer.Consume(MockContext(missingEvent)); }
        catch (InvalidOperationException) { /* expected */ }

        context.AuditLogs.Should().BeEmpty();
    }
}
