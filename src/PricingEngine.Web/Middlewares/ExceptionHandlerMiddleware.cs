using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using PricingEngine.Application.Exceptions;

namespace PricingEngine.Web.Middlewares;

/// <summary>
/// ASP.NET Core middleware that catches unhandled exceptions and maps them to structured JSON error responses.
/// Handles <see cref="ValidationException"/> (400), <see cref="ProductConfigurationNotFoundException"/> (404),
/// <see cref="UnsupportedProductException"/> (422), and all other exceptions (500).
/// </summary>
public class ExceptionHandlerMiddleware(RequestDelegate next)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    /// <summary>
    /// Invokes the middleware, delegating to the next component in the pipeline and catching any exception.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A task that completes when the response has been written.</returns>
    public async Task Invoke(HttpContext context)
    {
        try { await next(context); }
        catch (Exception ex) { await HandleExceptionAsync(context, ex); }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, body) = exception switch
        {
            ValidationException ve => (
                HttpStatusCode.BadRequest,
                (object)ve.Errors.Select(e => new { property = e.PropertyName, message = e.ErrorMessage })),
            ProductConfigurationNotFoundException => (
                HttpStatusCode.NotFound,
                (object)new[] { exception.Message }),
            UnsupportedProductException => (
                HttpStatusCode.UnprocessableEntity,
                (object)new[] { exception.Message }),
            _ => (
                HttpStatusCode.InternalServerError,
                (object)new[] { "An unexpected error occurred." }),
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsync(JsonSerializer.Serialize(body, JsonOptions));
    }
}

/// <summary>
/// Extension methods for registering <see cref="ExceptionHandlerMiddleware"/> in the request pipeline.
/// </summary>
public static class ExceptionHandlerMiddlewareExtensions
{
    /// <summary>
    /// Adds the <see cref="ExceptionHandlerMiddleware"/> to the application's request pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The same <paramref name="app"/> instance for chaining.</returns>
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionHandlerMiddleware>();
}
