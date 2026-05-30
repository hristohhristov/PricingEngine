using PricingEngine.Domain.Common;

namespace PricingEngine.Domain.Quotes.Events;

/// <summary>
/// Domain event raised when a new <c>QuoteRecord</c> is created.
/// Carries the full financial breakdown of the generated quote for downstream handlers.
/// </summary>
public class QuoteGeneratedDomainEvent : DomainEvent<Guid>
{
    /// <summary>
    /// Initialises the event with all quote financial details and sets the event identifier to the quote id.
    /// </summary>
    /// <param name="quoteId">The unique identifier of the generated quote.</param>
    /// <param name="productCode">The product code for which the quote was calculated.</param>
    /// <param name="quoteDate">The UTC date on which the quote was generated.</param>
    /// <param name="netPremium">The base premium before tax.</param>
    /// <param name="taxAmount">The tax levied on the net premium.</param>
    /// <param name="feeAmount">The flat product administration fee.</param>
    /// <param name="totalAmount">The total payable amount (net + tax + fee).</param>
    /// <param name="currency">The ISO 4217 currency code for all monetary amounts.</param>
    public QuoteGeneratedDomainEvent(
        Guid     quoteId,
        string   productCode,
        DateTime quoteDate,
        decimal  netPremium,
        decimal  taxAmount,
        decimal  feeAmount,
        decimal  totalAmount,
        string   currency)
    {
        QuoteId     = quoteId;
        ProductCode = productCode;
        QuoteDate   = quoteDate;
        NetPremium  = netPremium;
        TaxAmount   = taxAmount;
        FeeAmount   = feeAmount;
        TotalAmount = totalAmount;
        Currency    = currency;

        SetId(quoteId);
    }

    /// <summary>Gets the unique identifier of the generated quote.</summary>
    public Guid     QuoteId     { get; }

    /// <summary>Gets the product code for which the quote was calculated.</summary>
    public string   ProductCode { get; }

    /// <summary>Gets the UTC date on which the quote was generated.</summary>
    public DateTime QuoteDate   { get; }

    /// <summary>Gets the base premium amount before tax.</summary>
    public decimal  NetPremium  { get; }

    /// <summary>Gets the tax amount levied on the net premium.</summary>
    public decimal  TaxAmount   { get; }

    /// <summary>Gets the flat product administration fee.</summary>
    public decimal  FeeAmount   { get; }

    /// <summary>Gets the total payable amount (net premium + tax + fee).</summary>
    public decimal  TotalAmount { get; }

    /// <summary>Gets the ISO 4217 currency code for all monetary amounts.</summary>
    public string   Currency    { get; }
}
