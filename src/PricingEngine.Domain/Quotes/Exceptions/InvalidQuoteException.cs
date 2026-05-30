using System.Diagnostics.CodeAnalysis;
using PricingEngine.Domain.Common;

namespace PricingEngine.Domain.Quotes.Exceptions;

/// <summary>
/// Thrown when a <c>QuoteRecord</c> aggregate violates a domain invariant.
/// Examples include an invalid product code, a negative monetary amount, or an illegal status transition.
/// </summary>
[ExcludeFromCodeCoverage]
public class InvalidQuoteException : BaseDomainException
{
    /// <summary>Initialises a new instance with no message.</summary>
    public InvalidQuoteException() { }

    /// <summary>Initialises a new instance with the supplied error message.</summary>
    /// <param name="error">Description of the quote validation failure.</param>
    public InvalidQuoteException(string error) : base(error) { }
}
