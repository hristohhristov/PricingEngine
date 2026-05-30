using PricingEngine.Domain.Common.Models;
using PricingEngine.Domain.Pricing.Exceptions;

namespace PricingEngine.Domain.Pricing.Models;

/// <summary>
/// Value object representing a single installment payment plan for a quote.
/// Encapsulates count, amount per installment, total payable, and the surcharge relative to a single payment.
/// </summary>
public class InstallmentPlan : ValueObject
{
    private readonly int   _installmentCount;
    private readonly Money _amountPerInstallment;
    private readonly Money _totalAmount;
    private readonly Money _installmentSurchargeAmount;

    /// <summary>
    /// Initialises a new installment plan after validating the installment count.
    /// The surcharge is derived as the difference between <paramref name="totalAmount"/> and <paramref name="netPayable"/>.
    /// </summary>
    /// <param name="installmentCount">Number of installments; must be 1, 2, or 4.</param>
    /// <param name="amountPerInstallment">The amount due per individual installment.</param>
    /// <param name="totalAmount">Total amount payable including any surcharge.</param>
    /// <param name="netPayable">The base amount without surcharge (total without installment markup).</param>
    /// <exception cref="InvalidInstallmentPlanException">Thrown when <paramref name="installmentCount"/> is not 1, 2, or 4.</exception>
    public InstallmentPlan(
        int   installmentCount,
        Money amountPerInstallment,
        Money totalAmount,
        Money netPayable)
    {
        Guard.ForValidInstallmentCount<InvalidInstallmentPlanException>(
            installmentCount, nameof(InstallmentCount));

        _installmentCount           = installmentCount;
        _amountPerInstallment       = amountPerInstallment;
        _totalAmount                = totalAmount;
        _installmentSurchargeAmount = totalAmount.Subtract(netPayable);
    }

    /// <summary>Gets the number of installments in this plan.</summary>
    public int   InstallmentCount           => _installmentCount;

    /// <summary>Gets the monetary amount due per individual installment.</summary>
    public Money AmountPerInstallment       => _amountPerInstallment;

    /// <summary>Gets the total amount payable including any installment surcharge.</summary>
    public Money TotalAmount                => _totalAmount;

    /// <summary>Gets the extra amount charged for splitting the payment (total minus single-payment amount).</summary>
    public Money InstallmentSurchargeAmount => _installmentSurchargeAmount;
}
