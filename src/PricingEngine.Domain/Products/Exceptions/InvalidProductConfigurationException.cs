using System.Diagnostics.CodeAnalysis;
using PricingEngine.Domain.Common;

namespace PricingEngine.Domain.Products.Exceptions;

[ExcludeFromCodeCoverage]
public class InvalidProductConfigurationException : BaseDomainException
{
    public InvalidProductConfigurationException() { }
    public InvalidProductConfigurationException(string error) : base(error) { }
}
