using System.Diagnostics.CodeAnalysis;
using PricingEngine.Domain.Common;

namespace PricingEngine.Domain.Products.Exceptions;

/// <summary>
/// Thrown when a <c>ProductConfiguration</c> aggregate violates a domain invariant.
/// Examples include an invalid product code length or a validity window where ValidTo precedes ValidFrom.
/// </summary>
[ExcludeFromCodeCoverage]
public class InvalidProductConfigurationException : BaseDomainException
{
    /// <summary>Initialises a new instance with no message.</summary>
    public InvalidProductConfigurationException() { }

    /// <summary>Initialises a new instance with the supplied error message.</summary>
    /// <param name="error">Description of the configuration validation failure.</param>
    public InvalidProductConfigurationException(string error) : base(error) { }
}
