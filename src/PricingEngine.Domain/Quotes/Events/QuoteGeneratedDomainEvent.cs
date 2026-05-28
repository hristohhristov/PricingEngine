using PricingEngine.Domain.Common;

namespace PricingEngine.Domain.Quotes.Events;

public class QuoteGeneratedDomainEvent : DomainEvent<Guid>
{
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

    public Guid     QuoteId     { get; }
    public string   ProductCode { get; }
    public DateTime QuoteDate   { get; }
    public decimal  NetPremium  { get; }
    public decimal  TaxAmount   { get; }
    public decimal  FeeAmount   { get; }
    public decimal  TotalAmount { get; }
    public string   Currency    { get; }
}
