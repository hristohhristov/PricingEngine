using PricingEngine.Domain.Common;
using PricingEngine.Domain.Pricing.Models;
using PricingEngine.Domain.Quotes.Models;

namespace PricingEngine.Domain.Quotes.Factories;

public interface IQuoteRecordFactory : IFactory<QuoteRecord>
{
    IQuoteRecordFactory WithProductCode(string productCode);
    IQuoteRecordFactory WithQuoteDate(DateTime quoteDate);
    IQuoteRecordFactory WithQuoteResult(QuoteResult quoteResult);
    IQuoteRecordFactory WithCurrency(string currency);
}
