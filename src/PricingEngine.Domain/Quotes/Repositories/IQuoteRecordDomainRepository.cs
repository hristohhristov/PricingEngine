using PricingEngine.Domain.Common;
using PricingEngine.Domain.Quotes.Models;

namespace PricingEngine.Domain.Quotes.Repositories;

/// <summary>
/// Domain repository contract for <see cref="QuoteRecord"/> aggregate roots.
/// Extends <see cref="IDomainRepository{TEntity}"/> with a lookup by unique identifier.
/// </summary>
public interface IQuoteRecordDomainRepository : IDomainRepository<QuoteRecord>
{
    /// <summary>
    /// Retrieves a quote record by its unique identifier, or <c>null</c> if not found.
    /// </summary>
    /// <param name="id">The unique identifier of the quote to look up.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The matching <see cref="QuoteRecord"/>, or <c>null</c>.</returns>
    Task<QuoteRecord?> FindById(Guid id, CancellationToken cancellationToken = default);
}
