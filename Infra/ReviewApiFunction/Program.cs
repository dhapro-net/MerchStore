
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;

using Microsoft.Extensions.DependencyInjection;
using MerchStore.Domain.Interfaces;
using MerchStore.Infrastructure.Persistence.Repositories; // Ensure this namespace contains ProductRepository
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
        services.AddScoped<IOrderQueryRepository, OrderQueryRepository>();
        services.AddScoped<IOrderCommandRepository, OrderCommandRepository>();
        services.AddScoped<IProductQueryRepository, ProductQueryRepository>();
        services.AddScoped<IProductCommandRepository, ProductCommandRepository>();
        services.AddScoped(typeof(IQueryRepository<,>), typeof(QueryRepository<,>));
        services.AddScoped(typeof(ICommandRepository<,>), typeof(CommandRepository<,>));

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


