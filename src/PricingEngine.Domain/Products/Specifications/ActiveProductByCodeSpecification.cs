using System.Linq.Expressions;
using PricingEngine.Domain.Common;
using PricingEngine.Domain.Products.Models;

namespace PricingEngine.Domain.Products.Specifications;

public class ActiveProductByCodeSpecification : Specification<ProductConfiguration>
{
    private readonly string   _productCode;
    private readonly DateTime _date;

    public ActiveProductByCodeSpecification(string productCode, DateTime date)
    {
        _productCode = productCode;
        _date        = date.Date;
    }

    public override Expression<Func<ProductConfiguration, bool>> ToExpression()
        => config =>
            config.ProductCode == _productCode &&
            config.ValidFrom   <= _date &&
            config.ValidTo     >= _date;
}
