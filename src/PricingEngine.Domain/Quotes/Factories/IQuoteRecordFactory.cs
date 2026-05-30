using PricingEngine.Domain.Common;
using PricingEngine.Domain.Pricing.Models;
using PricingEngine.Domain.Quotes.Models;

namespace PricingEngine.Domain.Quotes.Factories;

/// <summary>
/// Fluent factory contract for building <see cref="QuoteRecord"/> aggregate roots.
/// All <c>With*</c> methods return the same factory instance to support method chaining.
/// </summary>
public interface IQuoteRecordFactory : IFactory<QuoteRecord>
{
    /// <summary>Sets the product code for the quote being built.</summary>
    /// <param name="productCode">The identifier of the product that was priced.</param>
    /// <returns>The same factory instance for chaining.</returns>
    IQuoteRecordFactory WithProductCode(string productCode);

    /// <summary>Sets the effective date of the quote.</summary>
    /// <param name="quoteDate">The UTC date on which the quote is calculated.</param>
    /// <returns>The same factory instance for chaining.</returns>
    IQuoteRecordFactory WithQuoteDate(DateTime quoteDate);

    /// <summary>Sets the financial result (net premium, tax, fee) for the quote.</summary>
    /// <param name="quoteResult">The calculated quote breakdown.</param>
    /// <returns>The same factory instance for chaining.</returns>
    IQuoteRecordFactory WithQuoteResult(QuoteResult quoteResult);

    /// <summary>Overrides the default currency (EUR) for the quote.</summary>
    /// <param name="currency">ISO 4217 currency code to use.</param>
    /// <returns>The same factory instance for chaining.</returns>
    IQuoteRecordFactory WithCurrency(string currency);
}
