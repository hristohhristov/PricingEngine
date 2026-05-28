using PricingEngine.Domain.Common.Models;

namespace PricingEngine.Domain.Pricing.Models;

public class QuoteSummary : ValueObject
{
    private readonly QuoteResult                 _quoteResult;
    private readonly IReadOnlyList<InstallmentPlan> _installmentPlans;

    public QuoteSummary(QuoteResult quoteResult, IReadOnlyList<InstallmentPlan> installmentPlans)
    {
        _quoteResult      = quoteResult;
        _installmentPlans = installmentPlans;
    }

    public QuoteResult                  QuoteResult      => _quoteResult;
    public IReadOnlyList<InstallmentPlan> InstallmentPlans => _installmentPlans;

    public InstallmentPlan? GetPlan(int installmentCount)
        => _installmentPlans.FirstOrDefault(p => p.InstallmentCount == installmentCount);
}
