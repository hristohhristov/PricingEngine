using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace PricingEngine.Web;

[ApiController]
[Route("[controller]")]
public abstract class ApiController : ControllerBase
{
    private IMediator? mediator;

    protected IMediator Mediator
        => mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

    protected async Task<ActionResult<TResult>> Send<TResult>(IRequest<TResult> request)
    {
        var result = await Mediator.Send(request);
        if (result == null) return NotFound();
        return Ok(result);
    }
}
