using PricingEngine.Application.Interfaces;

namespace PricingEngine.Infrastructure.Persistence;

public class UnitOfWork(PricingDbContext context) : IUnitOfWork
{
    public Task CommitAsync(CancellationToken ct = default)
        => context.SaveChangesAsync(ct);
}
