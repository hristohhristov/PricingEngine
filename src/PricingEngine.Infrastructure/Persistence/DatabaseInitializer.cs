using Microsoft.EntityFrameworkCore;

namespace PricingEngine.Infrastructure.Persistence;

public class DatabaseInitializer(PricingDbContext context)
{
    public Task InitializeAsync(CancellationToken ct = default)
        => context.Database.MigrateAsync(ct);
}
