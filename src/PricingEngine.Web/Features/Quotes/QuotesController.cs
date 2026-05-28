using Microsoft.AspNetCore.Mvc;
using PricingEngine.Application.Commands;
using PricingEngine.Application.Responses;
using PricingEngine.Web.Features.Quotes.Requests;

namespace PricingEngine.Web.Features.Quotes;

[Route("api/v1/quotes")]
public class QuotesController : ApiController
{
    [HttpPost]
    public Task<ActionResult<QuoteSummaryResponse>> Post([FromBody] CalculateQuoteRequest request)
        => Send(new CalculateQuoteCommand(request.ProductCode, request.Payload));
}
