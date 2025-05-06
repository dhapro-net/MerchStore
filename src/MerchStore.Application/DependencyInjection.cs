using Microsoft.Extensions.DependencyInjection;
using MerchStore.Application.Services.Implementations;
using MerchStore.Application.Services.Interfaces;
using MerchStore.Application.ShoppingCart.Interfaces;
using MerchStore.Application.ShoppingCart.Services;

namespace MerchStore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Existing service registration
        services.AddScoped<ICatalogService, CatalogService>();
        
        // Add shopping cart services
        services.AddScoped<IShoppingCartService, ShoppingCartService>();
        services.AddScoped<IShoppingCartQueryService, ShoppingCartQueryService>();

        return services;
    }
}