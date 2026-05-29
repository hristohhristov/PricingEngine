using PricingEngine.Domain.Pricing.Models;
using PricingEngine.Domain.Quotes.Exceptions;
using PricingEngine.Domain.Quotes.Models;

namespace PricingEngine.Domain.Quotes.Factories;

public class QuoteRecordFactory : IQuoteRecordFactory
{
    private string      _productCode  = default!;
    private DateTime    _quoteDate;
    private QuoteResult _quoteResult  = default!;
    private string      _currency     = "EUR";

    private bool _productCodeSet = false;
    private bool _quoteDateSet   = false;
    private bool _quoteResultSet = false;

    public IQuoteRecordFactory WithProductCode(string productCode)
    {
        _productCode    = productCode;
        _productCodeSet = true;
        return this;
    }

    public IQuoteRecordFactory WithQuoteDate(DateTime quoteDate)
    {
        _quoteDate    = quoteDate;
        _quoteDateSet = true;
        return this;
    }

    public IQuoteRecordFactory WithQuoteResult(QuoteResult quoteResult)
    {
        _quoteResult    = quoteResult;
        _quoteResultSet = true;
        return this;
    }

    public IQuoteRecordFactory WithCurrency(string currency)
    {
        _currency = currency;
        return this;
    }

    public QuoteRecord Build()
    {
        if (!_productCodeSet || !_quoteDateSet || !_quoteResultSet)
            throw new InvalidQuoteException(
                "ProductCode, QuoteDate, and QuoteResult are all required.");

        var id = Guid.NewGuid();

        return new QuoteRecord(
            id:          id,
            productCode: _productCode,
            quoteDate:   _quoteDate,
            netPremium:  _quoteResult.NetPremium.Amount,
            taxAmount:   _quoteResult.TaxAmount.Amount,
            feeAmount:   _quoteResult.FeeAmount.Amount,
            currency:    _currency);
    }
}
