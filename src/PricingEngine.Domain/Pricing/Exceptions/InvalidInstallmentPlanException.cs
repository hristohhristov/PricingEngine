using System.Diagnostics.CodeAnalysis;
using PricingEngine.Domain.Common;

namespace PricingEngine.Domain.Pricing.Exceptions;

[ExcludeFromCodeCoverage]
public class InvalidInstallmentPlanException : BaseDomainException
{
    public InvalidInstallmentPlanException() { }
    public InvalidInstallmentPlanException(string error) : base(error) { }
}
