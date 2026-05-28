using PricingEngine.Domain.Common;
using PricingEngine.Domain.Quotes.Models;

namespace PricingEngine.Domain.Quotes.Repositories;

public interface IQuoteRecordDomainRepository : IDomainRepository<QuoteRecord>
{
    Task<QuoteRecord?> FindById(Guid id, CancellationToken cancellationToken = default);
}
