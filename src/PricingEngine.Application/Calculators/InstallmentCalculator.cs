using PricingEngine.Domain.Pricing.Enums;
using PricingEngine.Domain.Pricing.Interfaces;
using PricingEngine.Domain.Pricing.Models;

namespace PricingEngine.Application.Calculators;

public sealed class InstallmentCalculator : IInstallmentCalculator
{
    private static readonly (int Count, decimal SurchargeRate)[] Plans =
    [
        ((int)InstallmentCount.One,  0.00m),
        ((int)InstallmentCount.Two,  0.02m),
        ((int)InstallmentCount.Four, 0.05m),
    ];

    public IReadOnlyList<InstallmentPlan> Calculate(QuoteResult quoteResult)
    {
        var netPayable = quoteResult.TotalAmount;
        return Plans.Select(p => CreatePlan(netPayable, p.Count, p.SurchargeRate)).ToList();
    }

    private static InstallmentPlan CreatePlan(Money netPayable, int count, decimal surchargeRate)
    {
        var surcharge       = netPayable.Percentage(surchargeRate).Round();
        var totalWithCharge = netPayable.Add(surcharge);
        var perInstallment  = totalWithCharge.MultiplyBy(1m / count).Round();
        return new InstallmentPlan(count, perInstallment, totalWithCharge, netPayable);
    }
}
