using Microsoft.Extensions.DependencyInjection;
using MediatR;
using System.Reflection;
using MerchStore.Application.ShoppingCart.Services;
using MerchStore.Application.ShoppingCart.Interfaces;
using MerchStore.Application.Catalog.Queries;
using MerchStore.Application.ShoppingCart.Handlers;
using MerchStore.Domain.Services;
using MerchStore.Application.Services.Implementations;
using MerchStore.Application.Services.Interfaces;


namespace MerchStore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(
            Assembly.GetExecutingAssembly(),
            typeof(DependencyInjection).Assembly,
            typeof(GetAllProductsQueryHandler).Assembly,
            typeof(AddCartCommandHandler).Assembly
        );
        
        services.AddScoped<IShoppingCartCommandService, ShoppingCartCommandService>();
        services.AddScoped<IShoppingCartQueryService, ShoppingCartQueryService>();
        services.AddScoped<ICatalogService, CatalogService>();
        services.AddScoped<IReviewService, ReviewService>();

        // leaving this here just in case it seems like a good idea to use later services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddScoped<CartCalculationService>();

        return services;
    }
}