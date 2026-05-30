namespace PricingEngine.Domain.Quotes.Enums;

/// <summary>
/// Represents the lifecycle state of a <c>QuoteRecord</c> aggregate root.
/// A quote starts as <see cref="Pending"/> and transitions to <see cref="Audited"/> after the audit consumer processes it.
/// </summary>
public enum QuoteStatus
{
    /// <summary>The quote has been calculated and saved but not yet audited.</summary>
    Pending = 0,

    /// <summary>The quote has been successfully processed by the audit consumer.</summary>
    Audited = 1
}
