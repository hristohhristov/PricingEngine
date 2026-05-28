using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using PricingEngine.Application.Exceptions;

namespace PricingEngine.Web.Middlewares;

public class ExceptionHandlerMiddleware(RequestDelegate next)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

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

public static class ExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionHandlerMiddleware>();
}
