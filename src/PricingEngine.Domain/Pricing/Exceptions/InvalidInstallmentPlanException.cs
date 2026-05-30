using System.Diagnostics.CodeAnalysis;
using PricingEngine.Domain.Common;

namespace PricingEngine.Domain.Pricing.Exceptions;

/// <summary>
/// Thrown when an <c>InstallmentPlan</c> is constructed with an invalid installment count.
/// Valid counts are 1, 2, and 4.
/// </summary>
[ExcludeFromCodeCoverage]
public class InvalidInstallmentPlanException : BaseDomainException
{
    /// <summary>Initialises a new instance with no message.</summary>
    public InvalidInstallmentPlanException() { }

    /// <summary>Initialises a new instance with the supplied error message.</summary>
    /// <param name="error">Description of the validation failure.</param>
    public InvalidInstallmentPlanException(string error) : base(error) { }
}
