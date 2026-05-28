using PricingEngine.Domain.Common;

namespace PricingEngine.Domain.Quotes.Events;

public class QuoteGeneratedIntegrationEvent : IIntegrationEvent
{
    public QuoteGeneratedIntegrationEvent()
    {
        EventId    = Guid.NewGuid();
        EventType  = "QuoteGenerated";
        Version    = "v1";
        OccurredAt = DateTime.UtcNow;
    }

    // IIntegrationEvent metadata
    public Guid     EventId    { get; set; }
    public string   EventType  { get; set; }
    public string   Version    { get; set; }
    public DateTime OccurredAt { get; set; }

    // Business payload for the audit consumer
    public Guid     QuoteId     { get; set; }
    public string   ProductCode { get; set; } = default!;
    public DateTime QuoteDate   { get; set; }
    public decimal  NetPremium  { get; set; }
    public decimal  TaxAmount   { get; set; }
    public decimal  FeeAmount   { get; set; }
    public decimal  TotalAmount { get; set; }
    public string   Currency    { get; set; } = default!;
}
