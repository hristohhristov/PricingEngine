using System.Diagnostics.CodeAnalysis;
using PricingEngine.Domain.Common;

namespace PricingEngine.Domain.Pricing.Exceptions;

[ExcludeFromCodeCoverage]
public class InvalidMoneyException : BaseDomainException
{
    public InvalidMoneyException() { }
    public InvalidMoneyException(string error) : base(error) { }
}
