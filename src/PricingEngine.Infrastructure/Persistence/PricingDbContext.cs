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

public class PricingDbContext(DbContextOptions<PricingDbContext> options) : DbContext(options)
{
    public DbSet<ProductConfiguration> ProductConfigurations => Set<ProductConfiguration>();
    public DbSet<QuoteRecord>          QuoteRecords          => Set<QuoteRecord>();
    public DbSet<AuditLog>             AuditLogs             => Set<AuditLog>();

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

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entity in ChangeTracker.Entries<IEntity>().Select(e => e.Entity))
            entity.ClearEvents();
        return await base.SaveChangesAsync(ct);
    }
}
