using Microsoft.Extensions.DependencyInjection;
using MerchStore.Application.Services.Interfaces;
using MediatR;
using System.Reflection;
using MerchStore.Application.ShoppingCart.Services;
using MerchStore.Application.ShoppingCart.Interfaces;
using MerchStore.Application.Catalog.Queries;
using MerchStore.Application.Services.Implementations;



namespace MerchStore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {


        services.AddMediatR(
            Assembly.GetExecutingAssembly(),
            typeof(DependencyInjection).Assembly,
            typeof(GetAllProductsQueryHandler).Assembly
        );
        services.AddScoped<IShoppingCartQueryService, ShoppingCartQueryService>();
        services.AddScoped<IShoppingCartService, ShoppingCartService>();
        
        services.AddScoped<ICatalogService, CatalogService>();
        services.AddScoped<IReviewService, ReviewService>();


        return services;
    }
}