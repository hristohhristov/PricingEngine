using MediatR;
using Microsoft.Extensions.Logging;
using PricingEngine.Application.Exceptions;
using PricingEngine.Application.Interfaces;
using PricingEngine.Application.Responses;
using PricingEngine.Application.Strategies;
using PricingEngine.Domain.Pricing.Interfaces;
using PricingEngine.Domain.Products.Repositories;
using PricingEngine.Domain.Quotes.Events;
using PricingEngine.Domain.Quotes.Factories;
using PricingEngine.Domain.Quotes.Repositories;

namespace PricingEngine.Application.Commands;

public class CalculateQuoteCommandHandler(
    IProductConfigurationDomainRepository productConfigRepo,
    IQuoteRecordDomainRepository quoteRepo,
    IQuoteRecordFactory quoteRecordFactory,
    PricingStrategyFactory strategyFactory,
    IInstallmentCalculator installmentCalculator,
    IIntegrationEventPublisher eventPublisher,
    IUnitOfWork unitOfWork,
    ILogger<CalculateQuoteCommandHandler> logger)
    : IRequestHandler<CalculateQuoteCommand, QuoteSummaryResponse>
{
    public async Task<QuoteSummaryResponse> Handle(
        CalculateQuoteCommand request, CancellationToken ct)
    {
        var config = await productConfigRepo
            .FindByProductCodeAndDate(request.ProductCode, DateTime.UtcNow, ct)
            ?? throw new ProductConfigurationNotFoundException(request.ProductCode);

        var strategy = strategyFactory.Resolve(request.ProductCode);

        var quoteResult      = strategy.Calculate(request.Payload, config.ConfigData);
        var installmentPlans = installmentCalculator.Calculate(quoteResult);

        var quoteRecord = quoteRecordFactory
            .WithProductCode(request.ProductCode)
            .WithQuoteDate(DateTime.UtcNow)
            .WithQuoteResult(quoteResult)
            .Build();

        await quoteRepo.Save(quoteRecord, ct);

       try
        {
            await eventPublisher.Publish(new QuoteGeneratedIntegrationEvent
            {
                QuoteId     = quoteRecord.Id,
                ProductCode = quoteRecord.ProductCode,
                QuoteDate   = quoteRecord.QuoteDate,
                NetPremium  = quoteRecord.NetPremium,
                TaxAmount   = quoteRecord.TaxAmount,
                FeeAmount   = quoteRecord.FeeAmount,
                TotalAmount = quoteRecord.TotalAmount,
                Currency    = quoteRecord.Currency,
            }, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "Failed to queue integration event for QuoteId {QuoteId}. " +
                "The quote will be saved but may not be audited automatically.",
                quoteRecord.Id);
        }

        await unitOfWork.CommitAsync(ct);

        return QuoteSummaryResponse.From(quoteRecord.Id, request.ProductCode,
            quoteResult, installmentPlans);
    }
}
