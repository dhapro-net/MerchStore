using Microsoft.Extensions.DependencyInjection;
using MerchStore.Application.Services.Implementations;
using MerchStore.Application.Services.Interfaces;
using MediatR;
using System.Reflection;
using MerchStore.Domain.ShoppingCart.Interfaces;
using MerchStore.Application.ShoppingCart.Services;
using MerchStore.Application.ShoppingCart.Interfaces;


namespace MerchStore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {


        services.AddScoped<ICatalogService, CatalogService>();
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddMediatR(typeof(DependencyInjection).Assembly);
        services.AddScoped<IShoppingCartQueryService, ShoppingCartQueryService>();
        services.AddScoped<IShoppingCartService, ShoppingCartService>();

        return services;
    }
}