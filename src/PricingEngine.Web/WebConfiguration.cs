using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace PricingEngine.Web;

/// <summary>
/// Extension methods that register Web-layer services with the DI container.
/// Configures MVC controllers, suppresses the default model-state filter, and registers Swagger.
/// </summary>
public static class WebConfiguration
{
    /// <summary>
    /// Adds all Web services (controllers, API behaviour options, and Swagger) to the service collection.
    /// </summary>
    /// <param name="services">The application's service collection.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddWeb(this IServiceCollection services)
    {
        services.AddControllers();

        services.Configure<ApiBehaviorOptions>(o => o.SuppressModelStateInvalidFilter = true);

        services.AddSwaggerGen(c =>
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "PricingEngine API", Version = "v1" }));

        return services;
    }
}
