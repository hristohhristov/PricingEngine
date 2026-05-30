using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using PricingEngine.Application.Responses;
using PricingEngine.Domain.Quotes.Enums;
using PricingEngine.Domain.Quotes.Events;
using PricingEngine.Tests.Integration.Helpers;
using PricingEngine.Tests.Integration.Infrastructure;

namespace PricingEngine.Tests.Integration.Quotes;

/// <summary>
/// End-to-end integration tests for <c>POST /api/v1/quotes</c>.
/// Each test runs against a real SQL Server container (via TestContainers),
/// a real MediatR + domain pipeline, and a mocked IIntegrationEventPublisher.
/// </summary>
public class CalculateQuoteIntegrationTests
    : IntegrationTestBase
{
    private const string Endpoint = "/api/v1/quotes";

    public CalculateQuoteIntegrationTests(PricingEngineWebApplicationFactory factory)
        : base(factory) { }

    // ── HOME_V1 happy path ────────────────────────────────────────────────────

    [Fact]
    public async Task HomeV1_ValidRequest_Returns200()
    {
        var response = await Http.PostAsync(Endpoint, QuoteRequestBuilder.ForHomeV1());

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task HomeV1_ValidRequest_ProductCodeIsHomeV1()
    {
        var response = await Http.PostAsync(Endpoint, QuoteRequestBuilder.ForHomeV1());
        var body     = await response.Content.ReadFromJsonAsync<QuoteSummaryResponse>();

        body!.ProductCode.Should().Be("HOME_V1");
    }

    [Fact]
    public async Task HomeV1_ValidRequest_BreakdownIsCorrect()
    {
        // insuredSum = 100 000, tariff = 0.5% → net = 500, tax = 50, fee = 25, total = 575
        var response = await Http.PostAsync(Endpoint, QuoteRequestBuilder.ForHomeV1(100_000m));
        var body     = await response.Content.ReadFromJsonAsync<QuoteSummaryResponse>();

        body!.Breakdown.NetPremium.Should().Be(500m);
        body.Breakdown.TaxAmount.Should().Be(50m);
        body.Breakdown.FeeAmount.Should().Be(25m);
        body.Breakdown.TotalAmount.Should().Be(575m);
        body.Breakdown.Currency.Should().Be("EUR");
    }

    [Fact]
    public async Task HomeV1_ValidRequest_ThreeInstallmentOptions()
    {
        var response = await Http.PostAsync(Endpoint, QuoteRequestBuilder.ForHomeV1());
        var body     = await response.Content.ReadFromJsonAsync<QuoteSummaryResponse>();

        body!.InstallmentOptions.Should().HaveCount(3);
    }

    [Fact]
    public async Task HomeV1_ValidRequest_QuoteRecordSavedWithPendingStatus()
    {
        await Http.PostAsync(Endpoint, QuoteRequestBuilder.ForHomeV1());

        await using var db = CreateDbContext();
        var records = await db.QuoteRecords.ToListAsync();

        records.Should().HaveCount(1);
        records[0].Status.Should().Be(QuoteStatus.Pending);
    }

    [Fact]
    public async Task HomeV1_ValidRequest_QuoteIdMatchesDbRecord()
    {
        var response = await Http.PostAsync(Endpoint, QuoteRequestBuilder.ForHomeV1());
        var body     = await response.Content.ReadFromJsonAsync<QuoteSummaryResponse>();

        await using var db     = CreateDbContext();
        var dbRecord           = await db.QuoteRecords.FirstAsync();

        body!.QuoteId.Should().Be(dbRecord.Id);
    }

    [Fact]
    public async Task HomeV1_ValidRequest_PublisherCalledOnce()
    {
        await Http.PostAsync(Endpoint, QuoteRequestBuilder.ForHomeV1());

        await Factory.EventPublisher
            .Received(1)
            .Publish(Arg.Any<QuoteGeneratedIntegrationEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HomeV1_InsuredSum200000_NetIs1000()
    {
        // 200 000 × 0.005 = 1 000, tax = 100, fee = 25, total = 1 125
        var response = await Http.PostAsync(Endpoint, QuoteRequestBuilder.ForHomeV1(200_000m));
        var body     = await response.Content.ReadFromJsonAsync<QuoteSummaryResponse>();

        body!.Breakdown.NetPremium.Should().Be(1_000m);
        body.Breakdown.TaxAmount.Should().Be(100m);
        body.Breakdown.TotalAmount.Should().Be(1_125m);
    }

    // ── AUTO_V1 happy path ────────────────────────────────────────────────────

    [Fact]
    public async Task AutoV1_ValidRequest_Returns200()
    {
        var response = await Http.PostAsync(Endpoint, QuoteRequestBuilder.ForAutoV1());

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AutoV1_ValidRequest_ProductCodeIsAutoV1()
    {
        var response = await Http.PostAsync(Endpoint, QuoteRequestBuilder.ForAutoV1());
        var body     = await response.Content.ReadFromJsonAsync<QuoteSummaryResponse>();

        body!.ProductCode.Should().Be("AUTO_V1");
    }

    [Fact]
    public async Task AutoV1_ValidRequest_AdminFeeIs40()
    {
        var response = await Http.PostAsync(Endpoint, QuoteRequestBuilder.ForAutoV1());
        var body     = await response.Content.ReadFromJsonAsync<QuoteSummaryResponse>();

        body!.Breakdown.FeeAmount.Should().Be(40m);
    }

    // ── Validation errors (400) ───────────────────────────────────────────────

    [Fact]
    public async Task EmptyProductCode_Returns400()
    {
        var response = await Http.PostAsync(Endpoint, QuoteRequestBuilder.EmptyProductCode());

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvalidPayload_Returns400WithMessage()
    {
        var response = await Http.PostAsync(Endpoint, QuoteRequestBuilder.InvalidPayload());
        var body     = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Should().Contain("Payload must be a valid JSON object");
    }

    // ── Not found (404) ───────────────────────────────────────────────────────

    [Fact]
    public async Task UnknownProductCode_Returns404()
    {
        // "UNKNOWN_V1" has no seeded ProductConfiguration in the DB.
        using var content = QuoteRequestBuilder.ForHomeV1();
        var json  = await content.ReadAsStringAsync();
        var withUnknown = new System.Net.Http.StringContent(
            json.Replace("HOME_V1", "UNKNOWN_V1"),
            System.Text.Encoding.UTF8,
            "application/json");

        var response = await Http.PostAsync(Endpoint, withUnknown);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
