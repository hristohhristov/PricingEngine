using PricingEngine.Domain.Common;

namespace PricingEngine.Domain.Quotes.Events;

/// <summary>
/// Integration event published to the message broker when a quote has been generated.
/// Consumed by the audit consumer to create an audit log entry and advance the quote's status.
/// </summary>
public class QuoteGeneratedIntegrationEvent : IIntegrationEvent
{
    /// <summary>Initialises the event envelope with a new identifier, type name, schema version, and current timestamp.</summary>
    public QuoteGeneratedIntegrationEvent()
    {
        EventId    = Guid.NewGuid();
        EventType  = "QuoteGenerated";
        Version    = "v1";
        OccurredAt = DateTime.UtcNow;
    }

    /// <summary>Gets or sets the unique identifier of this event instance; used for idempotency.</summary>
    public Guid     EventId    { get; set; }

    /// <summary>Gets or sets the logical event type name.</summary>
    public string   EventType  { get; set; }

    /// <summary>Gets or sets the schema version of this event.</summary>
    public string   Version    { get; set; }

    /// <summary>Gets or sets the UTC timestamp at which the event was raised.</summary>
    public DateTime OccurredAt { get; set; }

    /// <summary>Gets or sets the unique identifier of the generated quote.</summary>
    public Guid     QuoteId     { get; set; }

    /// <summary>Gets or sets the product code for which the quote was calculated.</summary>
    public string   ProductCode { get; set; } = default!;

    /// <summary>Gets or sets the UTC date on which the quote was generated.</summary>
    public DateTime QuoteDate   { get; set; }

    /// <summary>Gets or sets the base premium amount before tax.</summary>
    public decimal  NetPremium  { get; set; }

    /// <summary>Gets or sets the tax amount levied on the net premium.</summary>
    public decimal  TaxAmount   { get; set; }

    /// <summary>Gets or sets the flat product administration fee.</summary>
    public decimal  FeeAmount   { get; set; }

    /// <summary>Gets or sets the total payable amount (net premium + tax + fee).</summary>
    public decimal  TotalAmount { get; set; }

    /// <summary>Gets or sets the ISO 4217 currency code for all monetary amounts.</summary>
    public string   Currency    { get; set; } = default!;
}
