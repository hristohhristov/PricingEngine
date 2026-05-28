using System.Linq.Expressions;
using PricingEngine.Domain.Common;
using PricingEngine.Domain.Quotes.Models;

namespace PricingEngine.Domain.Quotes.Specifications;

public class QuoteByIdSpecification : Specification<QuoteRecord>
{
    private readonly Guid _id;

    public QuoteByIdSpecification(Guid id) => _id = id;

    public override Expression<Func<QuoteRecord, bool>> ToExpression()
        => quote => quote.Id == _id;
}
