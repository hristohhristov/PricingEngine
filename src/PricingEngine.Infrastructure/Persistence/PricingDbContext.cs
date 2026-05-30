using System.Linq.Expressions;
using System.Reflection;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using PricingEngine.Domain.Common;
using PricingEngine.Domain.Common.Models;
using PricingEngine.Domain.Products.Models;
using PricingEngine.Domain.Quotes.Models;
using PricingEngine.Infrastructure.Audit;

namespace PricingEngine.Infrastructure.Persistence;

/// <summary>
/// EF Core database context for the PricingEngine bounded context.
/// Applies fluent configurations, MassTransit outbox/inbox tables, and a global soft-delete query filter.
/// </summary>
public class PricingDbContext(DbContextOptions<PricingDbContext> options) : DbContext(options)
{
    /// <summary>Gets the <c>ProductConfigurations</c> entity set.</summary>
    public DbSet<ProductConfiguration> ProductConfigurations => Set<ProductConfiguration>();

    /// <summary>Gets the <c>QuoteRecords</c> entity set.</summary>
    public DbSet<QuoteRecord>          QuoteRecords          => Set<QuoteRecord>();

    /// <summary>Gets the <c>AuditLogs</c> entity set.</summary>
    public DbSet<AuditLog>             AuditLogs             => Set<AuditLog>();

    /// <summary>
    /// Applies all entity type configurations from this assembly, registers MassTransit outbox/inbox tables,
    /// and configures a global soft-delete query filter on all <see cref="Entity"/>-derived types.
    /// </summary>
    /// <param name="builder">The model builder used to configure the EF Core model.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        builder.AddInboxStateEntity();
        builder.AddOutboxStateEntity();
        builder.AddOutboxMessageEntity();

        foreach (var entityType in builder.Model.GetEntityTypes()
            .Where(t => typeof(Entity).IsAssignableFrom(t.ClrType) && !t.IsOwned()))
        {
            var param  = Expression.Parameter(entityType.ClrType);
            var prop   = Expression.Property(param, nameof(Entity.IsDeleted));
            var filter = Expression.Lambda(Expression.Equal(prop, Expression.Constant(false)), param);
            entityType.SetQueryFilter(filter);
        }
    }

    /// <summary>
    /// Clears pending domain events from all tracked entities before delegating to EF Core's save mechanism.
    /// </summary>
    /// <param name="ct">Token used to cancel the operation.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entity in ChangeTracker.Entries<IEntity>().Select(e => e.Entity))
            entity.ClearEvents();
        return await base.SaveChangesAsync(ct);
    }
}
