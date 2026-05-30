using PricingEngine.Domain.Pricing.Enums;
using PricingEngine.Domain.Pricing.Interfaces;
using PricingEngine.Domain.Pricing.Models;

namespace PricingEngine.Application.Calculators;
/// <summary>
/// Implements the IInstallmentCalculator interface to calculate installment plans based on a given QuoteResult. The calculator defines a set of installment plans with associated surcharge rates and computes the total amount payable for each plan, including the surcharge, as well as the amount per installment. The results are returned as a list of InstallmentPlan objects.
/// </summary>
public sealed class InstallmentCalculator : IInstallmentCalculator
{
    /// <summary>
    /// Defines the available installment plans and their corresponding surcharge rates. Each tuple contains the number of installments and the surcharge rate applied to the total amount for that plan.
    /// </summary>
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
