using System.Diagnostics.CodeAnalysis;
using PricingEngine.Domain.Common;

namespace PricingEngine.Domain.Quotes.Exceptions;

[ExcludeFromCodeCoverage]
public class InvalidQuoteException : BaseDomainException
{
    public InvalidQuoteException() { }
    public InvalidQuoteException(string error) : base(error) { }
}
