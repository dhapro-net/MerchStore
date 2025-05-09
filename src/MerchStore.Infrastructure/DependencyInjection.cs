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

        // Register ShoppingCart services (split into Query and Command services)
        services.AddScoped<IShoppingCartQueryService, ShoppingCartQueryService>();
        services.AddScoped<IShoppingCartCommandService, ShoppingCartCommandService>();

        return services;
    }
}