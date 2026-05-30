using PricingEngine.Domain.Pricing.Models;
using PricingEngine.Domain.Quotes.Exceptions;
using PricingEngine.Domain.Quotes.Models;

namespace PricingEngine.Domain.Quotes.Factories;

/// <summary>
/// Concrete fluent factory for constructing <see cref="QuoteRecord"/> aggregate roots.
/// Tracks which required fields have been set and throws if any are missing when <see cref="Build"/> is called.
/// </summary>
public class QuoteRecordFactory : IQuoteRecordFactory
{
    private string      _productCode  = default!;
    private DateTime    _quoteDate;
    private QuoteResult _quoteResult  = default!;
    private string      _currency     = "EUR";

    private bool _productCodeSet = false;
    private bool _quoteDateSet   = false;
    private bool _quoteResultSet = false;

    /// <inheritdoc/>
    public IQuoteRecordFactory WithProductCode(string productCode)
    {
        _productCode    = productCode;
        _productCodeSet = true;
        return this;
    }

    /// <inheritdoc/>
    public IQuoteRecordFactory WithQuoteDate(DateTime quoteDate)
    {
        _quoteDate    = quoteDate;
        _quoteDateSet = true;
        return this;
    }

    /// <inheritdoc/>
    public IQuoteRecordFactory WithQuoteResult(QuoteResult quoteResult)
    {
        _quoteResult    = quoteResult;
        _quoteResultSet = true;
        return this;
    }

    /// <inheritdoc/>
    public IQuoteRecordFactory WithCurrency(string currency)
    {
        _currency = currency;
        return this;
    }

    /// <summary>
    /// Validates all accumulated state and creates a new <see cref="QuoteRecord"/> instance with a generated GUID.
    /// </summary>
    /// <returns>A fully initialised <see cref="QuoteRecord"/> aggregate root in the <c>Pending</c> status.</returns>
    /// <exception cref="InvalidQuoteException">
    /// Thrown when any of <c>ProductCode</c>, <c>QuoteDate</c>, or <c>QuoteResult</c> have not been set.
    /// </exception>
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
