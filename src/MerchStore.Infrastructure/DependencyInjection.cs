using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MerchStore.Application.Common.Interfaces;
using MerchStore.Domain.Interfaces;
using MerchStore.Domain.ShoppingCart.Interfaces;
using MerchStore.Infrastructure.Persistence;
using MerchStore.Infrastructure.Persistence.Repositories;
using MerchStore.Infrastructure.ExternalServices.Reviews.Configurations;
using MerchStore.Infrastructure.ExternalServices.Reviews;
using MerchStore.Infrastructure.Repositories;
using MerchStore.Application.ShoppingCart.Interfaces;
using MerchStore.Application.ShoppingCart.Services;


namespace MerchStore.Infrastructure;


// Contains extension methods for registering Infrastructure layer services with the dependency injection container.
// This keeps all registration logic in one place and makes it reusable.

public static class DependencyInjection
{

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Call specific registration methods
        services.AddPersistenceServices(configuration);
        services.AddReviewServices(configuration);
        // Add calls to other infrastructure registration methods here if needed (e.g., file storage, email service)

        return services;
    }
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register DbContext with in-memory database
        // In a real application, you'd use a real database
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("MerchStoreDb"));

        // Register repositories
        services.AddScoped<IProductRepository, ProductRepository>();

        services.AddScoped<IOrderRepository, OrderRepository>();

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Repository Manager
        services.AddScoped<IRepositoryManager, RepositoryManager>();

        // Add logging services if not already added
        services.AddLogging();

        // Register DbContext seeder
        services.AddScoped<AppDbContextSeeder>();

        // Register HttpContextAccessor for cookie access
        services.AddHttpContextAccessor();

        // Register cookie-based shopping cart repository
        services.AddScoped<IShoppingCartRepository, CookieShoppingCartRepository>();

        services.AddScoped<IShoppingCartService, ShoppingCartService>();
        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddScoped<IOrderRepository, OrderRepository>();


        return services;
    }

    public static IServiceCollection AddReviewServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register External Api options
        services.Configure<ReviewApiOptions>(configuration.GetSection(ReviewApiOptions.SectionName));

        // Register HttpClient for ReviewApiClient
        services.AddHttpClient<ReviewApiClient>()
            .SetHandlerLifetime(TimeSpan.FromMinutes(5)); // Set a lifetime for the handler

        // Register the mock service
        services.AddSingleton<MockReviewService>();

        // Register the repository with the circuit breaker
        services.AddScoped<IReviewRepository, ExternalReviewRepository>();

        return services;
    }

    public static async Task SeedDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<AppDbContextSeeder>();

        await seeder.SeedAsync();
    }
}