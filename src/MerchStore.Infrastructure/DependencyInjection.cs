using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MerchStore.Application.Common.Interfaces;
using MerchStore.Domain.Interfaces;
using MerchStore.Domain.ShoppingCart.Interfaces;
using MerchStore.Infrastructure.Persistence;
using MerchStore.Infrastructure.Persistence.Repositories;
using MerchStore.Infrastructure.Repositories;
using MerchStore.Application.ShoppingCart.Interfaces;
using MerchStore.Application.ShoppingCart.Services;

namespace MerchStore.Infrastructure;

/// <summary>
/// Contains extension methods for registering Infrastructure layer services with the dependency injection container.
/// This keeps all registration logic in one place and makes it reusable.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Infrastructure layer services to the DI container
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <param name="configuration">The configuration for database connection strings</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register DbContext with in-memory database
        // In a real application, you'd use a real database
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("MerchStoreDb"));

        // Register repositories
        services.AddScoped<IOrderQueryRepository, OrderQueryRepository>();
        services.AddScoped<IOrderCommandRepository, OrderCommandRepository>();
        services.AddScoped<IProductQueryRepository, ProductQueryRepository>();
        services.AddScoped<IProductCommandRepository, ProductCommandRepository>();
        services.AddScoped(typeof(IQueryRepository<,>), typeof(QueryRepository<,>));
        services.AddScoped(typeof(ICommandRepository<,>), typeof(CommandRepository<,>));

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

        // Register cookie-based shopping cart repositories
        services.AddScoped<IShoppingCartQueryRepository, CookieShoppingCartRepository>();
        services.AddScoped<IShoppingCartCommandRepository, CookieShoppingCartRepository>();

        services.AddScoped<IShoppingCartService, ShoppingCartService>();
        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();




        return services;
    }

    /// <summary>
    /// Seeds the database with initial data.
    /// This is an extension method on IServiceProvider to allow it to be called from Program.cs.
    /// </summary>
    /// <param name="serviceProvider">The service provider to resolve dependencies</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public static async Task SeedDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<AppDbContextSeeder>();

        await seeder.SeedAsync();
    }
}