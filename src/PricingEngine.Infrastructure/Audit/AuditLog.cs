namespace PricingEngine.Infrastructure.Audit;

/// <summary>
/// Read-model entity that records the outcome of auditing a processed quote.
/// Persisted to the <c>AuditLogs</c> table whenever the <c>AuditQuoteConsumer</c> handles a <c>QuoteGeneratedIntegrationEvent</c>.
/// </summary>
public class AuditLog
{
    /// <summary>Gets or sets the unique identifier of this audit log entry.</summary>
    public Guid     Id          { get; set; }

    /// <summary>Gets or sets the identifier of the quote that was audited.</summary>
    public Guid     QuoteId     { get; set; }

    /// <summary>Gets or sets the product code of the audited quote.</summary>
    public string   ProductCode { get; set; } = default!;

    /// <summary>Gets or sets the UTC date on which the original quote was generated.</summary>
    public DateTime QuoteDate   { get; set; }

    /// <summary>Gets or sets the total premium amount recorded at the time of auditing.</summary>
    public decimal  TotalAmount { get; set; }

    /// <summary>Gets or sets the ISO 4217 currency code of the audited quote.</summary>
    public string   Currency    { get; set; } = default!;

    /// <summary>Gets or sets the UTC timestamp at which the audit log was created.</summary>
    public DateTime ProcessedAt { get; set; }

    /// <summary>Gets or sets the identifier of the integration event that triggered this audit.</summary>
    public Guid     EventId     { get; set; }
}
