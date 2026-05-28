using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace PricingEngine.Web;

public static class WebConfiguration
{
    public static IServiceCollection AddWeb(this IServiceCollection services)
    {
        services.AddControllers();

        services.Configure<ApiBehaviorOptions>(o => o.SuppressModelStateInvalidFilter = true);

        services.AddSwaggerGen(c =>
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "PricingEngine API", Version = "v1" }));

        return services;
    }
}
