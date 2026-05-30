using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PricingEngine.Domain.Quotes.Events;
using PricingEngine.Infrastructure.Audit;
using PricingEngine.Infrastructure.Persistence;

namespace PricingEngine.Infrastructure.Messaging.Consumers;

/// <summary>
/// MassTransit consumer that handles <see cref="QuoteGeneratedIntegrationEvent"/> messages.
/// Marks the corresponding <c>QuoteRecord</c> as audited and persists an <see cref="AuditLog"/> entry.
/// </summary>
public class AuditQuoteConsumer(
    PricingDbContext context,
    ILogger<AuditQuoteConsumer> logger)
    : IConsumer<QuoteGeneratedIntegrationEvent>
{
    /// <summary>
    /// Processes a <see cref="QuoteGeneratedIntegrationEvent"/>: locates the quote record, creates an audit log,
    /// and advances the quote status to <c>Audited</c>.
    /// </summary>
    /// <param name="consumeContext">The MassTransit consume context containing the integration event message.</param>
    /// <returns>A task that completes when the audit record has been saved.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no <c>QuoteRecord</c> matching the event's <c>QuoteId</c> is found in the database.
    /// </exception>
    public async Task Consume(ConsumeContext<QuoteGeneratedIntegrationEvent> consumeContext)
    {
        var msg = consumeContext.Message;

        var quote = await context.QuoteRecords
            .FirstOrDefaultAsync(q => q.Id == msg.QuoteId);

        if (quote is null)
        {
            logger.LogError(
                "QuoteRecord {QuoteId} not found during audit processing. " +
                "Message will be moved to the error queue.",
                msg.QuoteId);

            throw new InvalidOperationException(
                $"QuoteRecord {msg.QuoteId} not found during audit processing.");
        }

        context.AuditLogs.Add(new AuditLog
        {
            Id          = Guid.NewGuid(),
            QuoteId     = msg.QuoteId,
            ProductCode = msg.ProductCode,
            QuoteDate   = msg.QuoteDate,
            TotalAmount = msg.TotalAmount,
            Currency    = msg.Currency,
            ProcessedAt = DateTime.UtcNow,
            EventId     = msg.EventId,
        });

        quote.MarkAsAudited();

        await context.SaveChangesAsync();

        logger.LogInformation(
            "Quote {QuoteId} marked as Audited and AuditLog created.", msg.QuoteId);
    }
}
