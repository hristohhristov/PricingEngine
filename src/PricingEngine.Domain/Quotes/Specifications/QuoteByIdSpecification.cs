using System.Linq.Expressions;
using PricingEngine.Domain.Common;
using PricingEngine.Domain.Quotes.Models;

namespace PricingEngine.Domain.Quotes.Specifications;

/// <summary>
/// Specification that matches a <see cref="QuoteRecord"/> by its unique identifier.
/// </summary>
public class QuoteByIdSpecification : Specification<QuoteRecord>
{
    private readonly Guid _id;

    /// <summary>Initialises the specification with the identifier to match.</summary>
    /// <param name="id">The unique identifier of the target quote record.</param>
    public QuoteByIdSpecification(Guid id) => _id = id;

    /// <summary>
    /// Returns an expression that selects the quote record matching the stored identifier.
    /// </summary>
    /// <returns>A lambda suitable for LINQ-to-SQL translation by EF Core.</returns>
    public override Expression<Func<QuoteRecord, bool>> ToExpression()
        => quote => quote.Id == _id;
}
