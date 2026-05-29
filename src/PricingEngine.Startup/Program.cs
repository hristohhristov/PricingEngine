using PricingEngine.Application;
using PricingEngine.Domain;
using PricingEngine.Infrastructure;
using PricingEngine.Infrastructure.Persistence;
using PricingEngine.Infrastructure.Seeding;
using PricingEngine.Web;
using PricingEngine.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDomain()
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddWeb();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await initializer.InitializeAsync();

    var seeder = scope.ServiceProvider.GetRequiredService<IProductConfigurationSeeder>();
    await seeder.SeedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandling();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
