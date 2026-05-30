using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using PricingEngine.Application.Calculators;
using PricingEngine.Application.Commands;
using PricingEngine.Application.Exceptions;
using PricingEngine.Application.Interfaces;
using PricingEngine.Application.Strategies;
using PricingEngine.Domain.Pricing.Interfaces;
using PricingEngine.Domain.Pricing.Models;
using PricingEngine.Domain.Products.Factories;
using PricingEngine.Domain.Products.Models;
using PricingEngine.Domain.Products.Repositories;
using PricingEngine.Domain.Quotes.Events;
using PricingEngine.Domain.Quotes.Factories;
using PricingEngine.Domain.Quotes.Models;
using PricingEngine.Domain.Quotes.Repositories;

namespace PricingEngine.Tests.Unit.Application.Commands;

public class CalculateQuoteCommandHandlerTests
{
    // ── Helpers ───────────────────────────────────────────────────────────────

    private static ProductConfiguration MakeProductConfig()
    {
        return new ProductConfigurationFactory()
            .WithProductCode("HOME_V1")
            .WithConfigData("""{"baseTariff":0.005,"fixedFee":25.0}""")
            .WithValidFrom(new DateTime(2024, 1, 1))
            .WithValidTo(new DateTime(2100, 1, 1))
            .Build();
    }

    private static CalculateQuoteCommand ValidCommand()
    {
        var payload = JsonDocument.Parse("""{"insuredSum":100000}""").RootElement;
        return new CalculateQuoteCommand("HOME_V1", payload);
    }

    private static QuoteResult HomeResult()
        => new(new Money(500m, "EUR"), new Money(50m, "EUR"), new Money(25m, "EUR"));

    private (
        CalculateQuoteCommandHandler handler,
        IProductConfigurationDomainRepository productConfigRepo,
        IQuoteRecordDomainRepository quoteRepo,
        IProductPricingStrategy strategy,
        IInstallmentCalculator installmentCalculator,
        IIntegrationEventPublisher eventPublisher,
        IUnitOfWork unitOfWork,
        ILogger<CalculateQuoteCommandHandler> logger
    ) BuildSut(ProductConfiguration? config = null)
    {
        var productConfigRepo  = Substitute.For<IProductConfigurationDomainRepository>();
        var quoteRepo          = Substitute.For<IQuoteRecordDomainRepository>();
        var quoteRecordFactory = new QuoteRecordFactory();
        var strategy           = Substitute.For<IProductPricingStrategy>();

        // Configure BEFORE PricingStrategyFactory reads SupportedProductCode in its constructor
        strategy.SupportedProductCode.Returns("HOME_V1");
        strategy.Calculate(Arg.Any<JsonElement>(), Arg.Any<string>()).Returns(HomeResult());

        var strategyFactory    = new PricingStrategyFactory([strategy]);
        var installmentCalc    = Substitute.For<IInstallmentCalculator>();
        var eventPublisher     = Substitute.For<IIntegrationEventPublisher>();
        var unitOfWork         = Substitute.For<IUnitOfWork>();
        var logger             = Substitute.For<ILogger<CalculateQuoteCommandHandler>>();

        var installmentPlans = new InstallmentCalculator().Calculate(HomeResult());
        installmentCalc.Calculate(Arg.Any<QuoteResult>()).Returns(installmentPlans);

        productConfigRepo
            .FindByProductCodeAndDate(Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(config ?? MakeProductConfig());

        var handler = new CalculateQuoteCommandHandler(
            productConfigRepo, quoteRepo, quoteRecordFactory,
            strategyFactory, installmentCalc, eventPublisher, unitOfWork, logger);

        return (handler, productConfigRepo, quoteRepo, strategy, installmentCalc, eventPublisher, unitOfWork, logger);
    }

    // ── Happy path ────────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_ValidCommand_ReturnsQuoteSummaryResponse()
    {
        var (handler, _, _, _, _, _, _, _) = BuildSut();
        var result = await handler.Handle(ValidCommand(), CancellationToken.None);
        result.Should().NotBeNull();
        result.ProductCode.Should().Be("HOME_V1");
    }

    [Fact]
    public async Task Handle_ValidCommand_ResponseContainsCorrectProductCode()
    {
        var (handler, _, _, _, _, _, _, _) = BuildSut();
        var result = await handler.Handle(ValidCommand(), CancellationToken.None);
        result.ProductCode.Should().Be("HOME_V1");
    }

    [Fact]
    public async Task Handle_ValidCommand_ResponseHasThreeInstallmentOptions()
    {
        var (handler, _, _, _, _, _, _, _) = BuildSut();
        var result = await handler.Handle(ValidCommand(), CancellationToken.None);
        result.InstallmentOptions.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_ValidCommand_QuoteIdIsNotEmpty()
    {
        var (handler, _, _, _, _, _, _, _) = BuildSut();
        var result = await handler.Handle(ValidCommand(), CancellationToken.None);
        result.QuoteId.Should().NotBeEmpty();
    }

    // ── Repository interactions ───────────────────────────────────────────────

    [Fact]
    public async Task Handle_ValidCommand_SavesQuoteRecord()
    {
        var (handler, _, quoteRepo, _, _, _, _, _) = BuildSut();
        await handler.Handle(ValidCommand(), CancellationToken.None);
        await quoteRepo.Received(1).Save(Arg.Any<QuoteRecord>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidCommand_CommitsUnitOfWork()
    {
        var (handler, _, _, _, _, _, unitOfWork, _) = BuildSut();
        await handler.Handle(ValidCommand(), CancellationToken.None);
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    // ── Integration event ─────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_ValidCommand_PublishesIntegrationEvent()
    {
        var (handler, _, _, _, _, eventPublisher, _, _) = BuildSut();
        await handler.Handle(ValidCommand(), CancellationToken.None);
        await eventPublisher.Received(1)
            .Publish(Arg.Any<QuoteGeneratedIntegrationEvent>(), Arg.Any<CancellationToken>());
    }

    // ── Error resilience: publish failure ────────────────────────────────────

    [Fact]
    public async Task Handle_PublishThrows_CommitAsyncIsStillCalled()
    {
        var (handler, _, _, _, _, eventPublisher, unitOfWork, _) = BuildSut();
        eventPublisher
            .Publish(Arg.Any<QuoteGeneratedIntegrationEvent>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("bus down"));

        await handler.Handle(ValidCommand(), CancellationToken.None);

        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_PublishThrows_DoesNotRethrow()
    {
        var (handler, _, _, _, _, eventPublisher, _, _) = BuildSut();
        eventPublisher
            .Publish(Arg.Any<QuoteGeneratedIntegrationEvent>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("bus down"));

        // Must NOT throw — exception is swallowed, HTTP 200 still returned
        var act = async () => await handler.Handle(ValidCommand(), CancellationToken.None);
        await act.Should().NotThrowAsync();
    }

    // ── Product not found ─────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_ProductConfigNotFound_ThrowsProductConfigurationNotFoundException()
    {
        var (handler, productConfigRepo, _, _, _, _, _, _) = BuildSut();
        productConfigRepo
            .FindByProductCodeAndDate(Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns((ProductConfiguration?)null);

        var act = async () => await handler.Handle(ValidCommand(), CancellationToken.None);
        await act.Should().ThrowAsync<ProductConfigurationNotFoundException>();
    }
}
