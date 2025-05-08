using Microsoft.Extensions.DependencyInjection;
using MerchStore.Application.Services.Implementations;
using MerchStore.Application.Services.Interfaces;
using MediatR;
using System.Reflection;


namespace MerchStore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {


        services.AddScoped<ICatalogService, CatalogService>();
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddScoped<IShoppingCartService, ShoppingCartService>();
        services.AddScoped<IShoppingCartQueryService, ShoppingCartQueryService>();
        services.AddLogging();

        return services;
    }
}