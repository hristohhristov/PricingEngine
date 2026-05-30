using System.Linq.Expressions;
using PricingEngine.Domain.Common;
using PricingEngine.Domain.Products.Models;

namespace PricingEngine.Domain.Products.Specifications;

/// <summary>
/// Specification that matches a <see cref="ProductConfiguration"/> by its product code and a reference date
/// falling within its validity window.
/// </summary>
public class ActiveProductByCodeSpecification : Specification<ProductConfiguration>
{
    private readonly string   _productCode;
    private readonly DateTime _date;

    /// <summary>
    /// Initialises the specification with the product code to match and the effective date to test.
    /// </summary>
    /// <param name="productCode">The product identifier to filter by.</param>
    /// <param name="date">The date the configuration must cover; only the date part is used.</param>
    public ActiveProductByCodeSpecification(string productCode, DateTime date)
    {
        _productCode = productCode;
        _date        = date.Date;
    }

    /// <summary>
    /// Returns an expression that selects configurations matching the product code and validity window.
    /// </summary>
    /// <returns>A lambda suitable for LINQ-to-SQL translation by EF Core.</returns>
    public override Expression<Func<ProductConfiguration, bool>> ToExpression()
        => config =>
            config.ProductCode == _productCode &&
            config.ValidFrom   <= _date &&
            config.ValidTo     >= _date;
}
