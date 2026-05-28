using MassTransit;
using Microsoft.EntityFrameworkCore;
using PricingEngine.Domain.Quotes.Events;
using PricingEngine.Infrastructure.Audit;
using PricingEngine.Infrastructure.Persistence;

namespace PricingEngine.Infrastructure.Messaging.Consumers;

public class AuditQuoteConsumer(PricingDbContext context)
    : IConsumer<QuoteGeneratedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<QuoteGeneratedIntegrationEvent> consumeContext)
    {
        var msg = consumeContext.Message;

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

        var quote = await context.QuoteRecords.FirstOrDefaultAsync(q => q.Id == msg.QuoteId);
        quote?.MarkAsAudited();

        await context.SaveChangesAsync();
    }
}
