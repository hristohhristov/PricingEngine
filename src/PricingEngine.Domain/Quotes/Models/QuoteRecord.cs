using PricingEngine.Domain.Common;
using PricingEngine.Domain.Common.Models;
using PricingEngine.Domain.Quotes.Enums;
using PricingEngine.Domain.Quotes.Events;
using PricingEngine.Domain.Quotes.Exceptions;

namespace PricingEngine.Domain.Quotes.Models;

using static ModelConstants.Quote;

/// <summary>
/// Aggregate root representing a calculated insurance quote.
/// State transitions (e.g., <see cref="MarkAsAudited"/>) are the only allowed mutations; all fields are read-only after construction.
/// </summary>
public class QuoteRecord : Entity<Guid>, IAggregateRoot
{
    /// <summary>Private parameterless constructor required by EF Core.</summary>
    private QuoteRecord() { }

    /// <summary>
    /// Creates a new quote record, validates all financial fields, and raises a <see cref="QuoteGeneratedDomainEvent"/>.
    /// </summary>
    /// <param name="id">The unique identifier for this quote.</param>
    /// <param name="productCode">The product code for which the quote was calculated.</param>
    /// <param name="quoteDate">The UTC date on which the quote was generated.</param>
    /// <param name="netPremium">Base premium before tax; must be non-negative.</param>
    /// <param name="taxAmount">Tax levied on the net premium; must be non-negative.</param>
    /// <param name="feeAmount">Flat product administration fee; must be non-negative.</param>
    /// <param name="currency">ISO 4217 currency code; must not be empty.</param>
    /// <exception cref="InvalidQuoteException">Thrown when any domain invariant is violated.</exception>
    internal QuoteRecord(
        Guid id,
        string productCode,
        DateTime quoteDate,
        decimal netPremium,
        decimal taxAmount,
        decimal feeAmount,
        string currency)
    {
        Guard.ForStringLength<InvalidQuoteException>(
            productCode, MinProductCodeLength, MaxProductCodeLength, nameof(ProductCode));
        Guard.AgainstNegativeAmount<InvalidQuoteException>(netPremium, nameof(NetPremium));
        Guard.AgainstNegativeAmount<InvalidQuoteException>(taxAmount, nameof(TaxAmount));
        Guard.AgainstNegativeAmount<InvalidQuoteException>(feeAmount, nameof(FeeAmount));
        Guard.AgainstEmptyString<InvalidQuoteException>(currency, nameof(Currency));

        SetId(id);
        ProductCode = productCode;
        QuoteDate = quoteDate;
        NetPremium = netPremium;
        TaxAmount = taxAmount;
        FeeAmount = feeAmount;
        Currency = currency;
        Status = QuoteStatus.Pending;

        RaiseEvent(new QuoteGeneratedDomainEvent(
            quoteId: id,
            productCode: productCode,
            quoteDate: quoteDate,
            netPremium: netPremium,
            taxAmount: taxAmount,
            feeAmount: feeAmount,
            totalAmount: netPremium + taxAmount + feeAmount,
            currency: currency));
    }

    /// <summary>Gets the product code for which this quote was calculated.</summary>
    public string ProductCode { get; private set; } = default!;

    /// <summary>Gets the UTC date on which this quote was generated.</summary>
    public DateTime QuoteDate { get; private set; }

    /// <summary>Gets the base premium amount before tax.</summary>
    public decimal NetPremium { get; private set; }

    /// <summary>Gets the tax amount levied on the net premium.</summary>
    public decimal TaxAmount { get; private set; }

    /// <summary>Gets the flat product administration fee.</summary>
    public decimal FeeAmount { get; private set; }

    /// <summary>Gets the ISO 4217 currency code for all monetary amounts on this quote.</summary>
    public string Currency { get; private set; } = default!;

    /// <summary>Gets the current lifecycle status of this quote.</summary>
    public QuoteStatus Status { get; private set; }

    /// <summary>Gets the total payable amount computed as the sum of net premium, tax, and fee.</summary>
    public decimal TotalAmount => NetPremium + TaxAmount + FeeAmount;

    /// <summary>
    /// Transitions this quote from <see cref="QuoteStatus.Pending"/> to <see cref="QuoteStatus.Audited"/>.
    /// </summary>
    /// <exception cref="InvalidQuoteException">Thrown when the quote is not in the <c>Pending</c> status.</exception>
    public void MarkAsAudited()
    {
        if (Status != QuoteStatus.Pending)
            throw new InvalidQuoteException("Only a Pending quote can be marked as Audited.");
        Status = QuoteStatus.Audited;
    }
}
