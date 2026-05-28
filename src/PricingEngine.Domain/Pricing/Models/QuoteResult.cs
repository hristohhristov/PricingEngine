using PricingEngine.Domain.Common.Models;

namespace PricingEngine.Domain.Pricing.Models;

public class QuoteResult : ValueObject
{
    private readonly Money _netPremium;
    private readonly Money _taxAmount;
    private readonly Money _feeAmount;

    public QuoteResult(Money netPremium, Money taxAmount, Money feeAmount)
    {
        _netPremium = netPremium;
        _taxAmount  = taxAmount;
        _feeAmount  = feeAmount;
    }

    public Money NetPremium => _netPremium;
    public Money TaxAmount  => _taxAmount;
    public Money FeeAmount  => _feeAmount;

    // Computed from the three components — not a backing field, so excluded
    // from ValueObject reflection-based equality. Equality is on the three components.
    public Money TotalAmount => _netPremium.Add(_taxAmount).Add(_feeAmount);
}
