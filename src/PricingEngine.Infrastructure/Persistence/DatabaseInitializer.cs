using Microsoft.EntityFrameworkCore;

namespace PricingEngine.Infrastructure.Persistence;

/// <summary>
/// Applies any pending EF Core migrations to the database at application startup.
/// Should be resolved from a scoped DI scope during the startup sequence.
/// </summary>
public class DatabaseInitializer(PricingDbContext context)
{
    /// <summary>
    /// Runs all pending migrations against the database asynchronously.
    /// </summary>
    /// <param name="ct">Token used to cancel the operation.</param>
    /// <returns>A task that completes when all migrations have been applied.</returns>
    public Task InitializeAsync(CancellationToken ct = default)
        => context.Database.MigrateAsync(ct);
}
