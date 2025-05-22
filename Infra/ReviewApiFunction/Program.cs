
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using MerchStore.Domain.Interfaces;
using MerchStore.Infrastructure.Persistence.Repositories;
using MerchStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using ReviewApiFunction;

var host = new HostBuilder()
   .ConfigureFunctionsWebApplication()
   .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        //services.ConfigureApplicationInsights(); // Removed as the method is not defined
        // Add OpenAPI configuration using our custom classes
        services.AddSingleton<IOpenApiConfigurationOptions, SwaggerConfiguration>();

        // Add custom UI options
        services.AddSingleton<IOpenApiCustomUIOptions, SwaggerUIConfiguration>();
        // ✅ In-memory database (you can replace with UseSqlServer later)
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("MerchStoreDb"));

        // ✅ Register Repositories
        services.AddScoped<IProductCommandRepository, EfProductCommandRepository>();
        services.AddScoped<IProductQueryRepository, EfProductQueryRepository>();
       // services.AddScoped<IProductRepository, ProductRepository>();


        // ✅ Register Seeder
        services.AddScoped<AppDbContextSeeder>();
    })
    .Build();

// ✅ Call seeder to insert products at startup
using (var scope = host.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<AppDbContextSeeder>();
    await seeder.SeedAsync(); // 👈 this ensures products are there
}

host.Run();


