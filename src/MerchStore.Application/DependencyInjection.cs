using Microsoft.Extensions.DependencyInjection;
using MerchStore.Application.Services.Implementations;
using MerchStore.Application.Services.Interfaces;
using MerchStore.Application.Services;

namespace MerchStore.Application;

public static class DependencyInjection
{

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {

        services.AddScoped<ICatalogService, CatalogService>();

        services.AddScoped<IShoppingCartService, ShoppingCartService>();
        services.AddScoped<IShoppingCartQueryService, ShoppingCartQueryService>();


        return services;
    }
}