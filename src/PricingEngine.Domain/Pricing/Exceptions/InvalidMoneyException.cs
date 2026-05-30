using System.Diagnostics.CodeAnalysis;
using PricingEngine.Domain.Common;

namespace PricingEngine.Domain.Pricing.Exceptions;

/// <summary>
/// Thrown when a <c>Money</c> value object is constructed or operated on with invalid parameters.
/// Examples include a negative amount, an empty currency code, or cross-currency arithmetic.
/// </summary>
[ExcludeFromCodeCoverage]
public class InvalidMoneyException : BaseDomainException
{
    /// <summary>Initialises a new instance with no message.</summary>
    public InvalidMoneyException() { }

    /// <summary>Initialises a new instance with the supplied error message.</summary>
    /// <param name="error">Description of the monetary validation failure.</param>
    public InvalidMoneyException(string error) : base(error) { }
}
