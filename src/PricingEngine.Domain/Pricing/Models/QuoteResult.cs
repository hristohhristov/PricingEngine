using PricingEngine.Domain.Common.Models;

namespace PricingEngine.Domain.Pricing.Models;

/// <summary>
/// Immutable value object that captures the financial breakdown of a calculated quote.
/// Exposes net premium, tax, and fee amounts along with a computed total.
/// </summary>
public class QuoteResult : ValueObject
{
    private readonly Money _netPremium;
    private readonly Money _taxAmount;
    private readonly Money _feeAmount;

    /// <summary>Initialises a new quote result from the three financial components.</summary>
    /// <param name="netPremium">The base premium before tax.</param>
    /// <param name="taxAmount">The tax amount calculated on the net premium.</param>
    /// <param name="feeAmount">Any flat product fee added to the total.</param>
    public QuoteResult(Money netPremium, Money taxAmount, Money feeAmount)
    {
        _netPremium = netPremium;
        _taxAmount  = taxAmount;
        _feeAmount  = feeAmount;
    }

    /// <summary>Gets the base premium amount before tax.</summary>
    public Money NetPremium => _netPremium;

    /// <summary>Gets the tax amount levied on the net premium.</summary>
    public Money TaxAmount  => _taxAmount;

    /// <summary>Gets the flat product administration fee.</summary>
    public Money FeeAmount  => _feeAmount;

    /// <summary>
    /// Gets the total payable amount computed as the sum of net premium, tax, and fee.
    /// This property is computed on the fly and is intentionally excluded from value-object equality.
    /// </summary>
    public Money TotalAmount => _netPremium.Add(_taxAmount).Add(_feeAmount);
}
