using PricingEngine.Domain.Common.Models;
using PricingEngine.Domain.Pricing.Exceptions;

namespace PricingEngine.Domain.Pricing.Models;

public class InstallmentPlan : ValueObject
{
    private readonly int   _installmentCount;
    private readonly Money _amountPerInstallment;
    private readonly Money _totalAmount;
    private readonly Money _installmentSurchargeAmount;

    public InstallmentPlan(
        int   installmentCount,
        Money amountPerInstallment,
        Money totalAmount,
        Money netPayable)
    {
        Guard.ForValidInstallmentCount<InvalidInstallmentPlanException>(
            installmentCount, nameof(InstallmentCount));

        _installmentCount         = installmentCount;
        _amountPerInstallment     = amountPerInstallment;
        _totalAmount              = totalAmount;
        _installmentSurchargeAmount = totalAmount.Subtract(netPayable);
    }

    public int   InstallmentCount           => _installmentCount;
    public Money AmountPerInstallment       => _amountPerInstallment;
    public Money TotalAmount                => _totalAmount;
    public Money InstallmentSurchargeAmount => _installmentSurchargeAmount;
}
