namespace PricingEngine.Infrastructure.Seeding;

/// <summary>
/// Contract for seeding default product configurations into the database at application startup.
/// Implementations check whether a configuration already exists before inserting.
/// </summary>
public interface IProductConfigurationSeeder
{
    /// <summary>
    /// Seeds all registered product definitions that do not yet have a configuration in the database.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when all seed operations have finished.</returns>
    Task SeedAsync(CancellationToken cancellationToken = default);
}
