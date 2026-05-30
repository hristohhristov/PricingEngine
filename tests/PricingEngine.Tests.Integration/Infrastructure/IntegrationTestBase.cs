using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using PricingEngine.Infrastructure.Persistence;

namespace PricingEngine.Tests.Integration.Infrastructure;

/// <summary>
/// Abstract base for integration tests. Concrete classes declare
/// <c>IClassFixture&lt;PricingEngineWebApplicationFactory&gt;</c>.
///
/// Before each test:
///   - Truncates volatile tables (QuoteRecords, AuditLogs, outbox tables).
///   - Clears all received calls on the EventPublisher mock.
///
/// ProductConfigurations is NOT cleared — seeded once at container startup.
/// </summary>
public abstract class IntegrationTestBase
    : IClassFixture<PricingEngineWebApplicationFactory>, IAsyncLifetime
{
    protected readonly HttpClient Http;
    protected readonly PricingEngineWebApplicationFactory Factory;

    protected IntegrationTestBase(PricingEngineWebApplicationFactory factory)
    {
        Factory = factory;
        Http    = factory.CreateClient();
    }

    // ── IAsyncLifetime ────────────────────────────────────────────────────────

    public async Task InitializeAsync()
    {
        // Clear tables that accumulate data across tests.
        await using var conn = new SqlConnection(Factory.ConnectionString);
        await conn.OpenAsync();

        // Outbox / inbox tables added by MassTransit EF Core outbox.
        foreach (var table in new[]
        {
            "OutboxMessage",
            "OutboxState",
            "InboxState",
            "AuditLogs",
            "QuoteRecords",
        })
        {
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = $"DELETE FROM [{table}]";
            try { await cmd.ExecuteNonQueryAsync(); }
            catch (SqlException) { /* table may not exist in every migration revision */ }
        }

        // Reset mock call history so every test starts with a clean slate.
        Factory.EventPublisher.ClearReceivedCalls();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Opens a fresh, scoped <see cref="PricingDbContext"/> for post-request
    /// database assertions. Caller is responsible for disposing.
    /// </summary>
    protected PricingDbContext CreateDbContext()
    {
        var scope = Factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<PricingDbContext>();
    }
}
