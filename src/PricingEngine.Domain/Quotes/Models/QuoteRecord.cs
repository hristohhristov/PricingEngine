using PricingEngine.Domain.Common;
using PricingEngine.Domain.Common.Models;
using PricingEngine.Domain.Quotes.Enums;
using PricingEngine.Domain.Quotes.Events;
using PricingEngine.Domain.Quotes.Exceptions;

namespace PricingEngine.Domain.Quotes.Models;

using static ModelConstants.Quote;

public class QuoteRecord : Entity<Guid>, IAggregateRoot
{
    private QuoteRecord() { }

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

    public string ProductCode { get; private set; } = default!;
    public DateTime QuoteDate { get; private set; }
    public decimal NetPremium { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal FeeAmount { get; private set; }
    public string Currency { get; private set; } = default!;
    public QuoteStatus Status { get; private set; }

    public decimal TotalAmount => NetPremium + TaxAmount + FeeAmount;

    public void MarkAsAudited()
    {
        if (Status != QuoteStatus.Pending)
            throw new InvalidQuoteException("Only a Pending quote can be marked as Audited.");
        Status = QuoteStatus.Audited;
    }
}
