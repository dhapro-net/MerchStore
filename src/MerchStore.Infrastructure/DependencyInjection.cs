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
using MongoDB.Driver;


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
        var provider = configuration["Config:DatabaseType"] ?? "EfCore";

        if (provider == "Mongo")
        {

            // Register MongoDB client and database
            services.AddSingleton<IMongoClient>(sp =>
            {
                var settings = MongoClientSettings.FromConnectionString(configuration["CosmosDb:ConnectionString"]);
                return new MongoClient(settings);
            });
            services.AddScoped<IMongoDatabase>(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                return client.GetDatabase(configuration["CosmosDb:DatabaseName"]);
            });

            // Register MongoDB repositories
            services.AddScoped<IProductQueryRepository, MongoProductQueryRepository>();
            services.AddScoped<IProductCommandRepository, MongoProductCommandRepository>();

            // Register MongoDB seeder
            services.AddScoped<MongoDbSeeder>();
        }
        else
        {
            // Register DbContext with in-memory database or your real provider
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("MerchStoreDb"));

            // Register EF Core repositories
            services.AddScoped<IOrderQueryRepository, OrderQueryRepository>();
            services.AddScoped<IOrderCommandRepository, OrderCommandRepository>();
            services.AddScoped<IProductQueryRepository, EfProductQueryRepository>();
            services.AddScoped<IProductCommandRepository, EfProductCommandRepository>();
            services.AddScoped(typeof(IQueryRepository<,>), typeof(QueryRepository<,>));
            services.AddScoped(typeof(ICommandRepository<,>), typeof(CommandRepository<,>));

            // Register Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register Repository Manager
            services.AddScoped<IRepositoryManager, RepositoryManager>();

            // Register EF Core seeder
            services.AddScoped<AppDbContextSeeder>();
        }

        // Register cookie-based shopping cart repositories (if not persistence-specific)
        services.AddScoped<IShoppingCartQueryRepository, CookieShoppingCartRepository>();
        services.AddScoped<IShoppingCartCommandRepository, CookieShoppingCartRepository>();

        // Register ShoppingCart services
        services.AddScoped<IShoppingCartQueryService, ShoppingCartQueryService>();
        services.AddScoped<IShoppingCartCommandService, ShoppingCartCommandService>();

        // Add logging services if not already added
        services.AddLogging();

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

    public static async Task SeedDatabaseAsync(this IServiceProvider serviceProvider, IConfiguration configuration)
    {
        using var scope = serviceProvider.CreateScope();
        var provider = configuration["Config:PersistenceProvider"] ?? "EfCore";

        if (provider == "Mongo")
        {
            var seeder = scope.ServiceProvider.GetRequiredService<MongoDbSeeder>();
            await seeder.SeedAsync();
        }
        else
        {
            var seeder = scope.ServiceProvider.GetRequiredService<AppDbContextSeeder>();
            await seeder.SeedAsync();
        }
    }
}