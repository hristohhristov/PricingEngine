using Microsoft.EntityFrameworkCore;
using PricingEngine.Domain.Quotes.Models;
using PricingEngine.Domain.Quotes.Repositories;
using PricingEngine.Domain.Quotes.Specifications;

namespace PricingEngine.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IQuoteRecordDomainRepository"/>.
/// Uses <see cref="QuoteByIdSpecification"/> for the identifier-based lookup query.
/// </summary>
public class QuoteRecordRepository(PricingDbContext context)
    : DataRepository<PricingDbContext, QuoteRecord>(context),
      IQuoteRecordDomainRepository
{
    /// <summary>
    /// Returns the <see cref="QuoteRecord"/> matching the given identifier, or <c>null</c> when not found.
    /// </summary>
    /// <param name="id">The unique identifier of the quote record.</param>
    /// <param name="ct">Token used to cancel the operation.</param>
    /// <returns>The matching <see cref="QuoteRecord"/>, or <c>null</c>.</returns>
    public Task<QuoteRecord?> FindById(Guid id, CancellationToken ct = default)
        => Context.QuoteRecords
            .Where(new QuoteByIdSpecification(id))
            .FirstOrDefaultAsync(ct);
}
