using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace PricingEngine.Web;

/// <summary>
/// Abstract base controller that provides a lazy-resolved <see cref="IMediator"/> instance
/// and a convenience <see cref="Send{TResult}"/> method for dispatching MediatR requests.
/// </summary>
[ApiController]
[Route("[controller]")]
public abstract class ApiController : ControllerBase
{
    private IMediator? mediator;

    /// <summary>Gets the MediatR mediator resolved lazily from the current request's service provider.</summary>
    protected IMediator Mediator
        => mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

    /// <summary>
    /// Sends a MediatR request and wraps the result in a 200 OK response,
    /// or returns 404 Not Found when the result is <c>null</c>.
    /// </summary>
    /// <typeparam name="TResult">The response type produced by the MediatR handler.</typeparam>
    /// <param name="request">The MediatR request to dispatch.</param>
    /// <returns>200 OK with the handler result, or 404 Not Found when the result is <c>null</c>.</returns>
    protected async Task<ActionResult<TResult>> Send<TResult>(IRequest<TResult> request)
    {
        var result = await Mediator.Send(request);
        if (result == null) return NotFound();
        return Ok(result);
    }
}
