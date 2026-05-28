using Microsoft.EntityFrameworkCore;
using PricingEngine.Domain.Quotes.Models;
using PricingEngine.Domain.Quotes.Repositories;
using PricingEngine.Domain.Quotes.Specifications;

namespace PricingEngine.Infrastructure.Persistence.Repositories;

public class QuoteRecordRepository(PricingDbContext context)
    : DataRepository<PricingDbContext, QuoteRecord>(context),
      IQuoteRecordDomainRepository
{
    public Task<QuoteRecord?> FindById(Guid id, CancellationToken ct = default)
        => Context.QuoteRecords
            .Where(new QuoteByIdSpecification(id))
            .FirstOrDefaultAsync(ct);
}
