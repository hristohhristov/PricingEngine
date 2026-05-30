using Microsoft.AspNetCore.Mvc;
using PricingEngine.Application.Commands;
using PricingEngine.Application.Responses;
using PricingEngine.Web.Features.Quotes.Requests;

namespace PricingEngine.Web.Features.Quotes;

/// <summary>
/// REST controller exposing the quote calculation endpoint at <c>POST /api/v1/quotes</c>.
/// </summary>
[Route("api/v1/quotes")]
public class QuotesController : ApiController
{
    /// <summary>
    /// Calculates a premium quote for the specified product and returns a full breakdown with installment options.
    /// </summary>
    /// <param name="request">The request body containing the product code and product-specific JSON payload.</param>
    /// <returns>
    /// 200 OK with a <see cref="QuoteSummaryResponse"/>, 400 Bad Request for validation errors,
    /// 404 Not Found when no active product configuration exists, or 422 Unprocessable Entity for unsupported products.
    /// </returns>
    [HttpPost]
    public Task<ActionResult<QuoteSummaryResponse>> Post([FromBody] CalculateQuoteRequest request)
        => Send(new CalculateQuoteCommand(request.ProductCode, request.Payload));
}
