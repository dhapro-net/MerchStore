using Microsoft.Extensions.DependencyInjection;
using MerchStore.Application.Services.Implementations;
using MerchStore.Application.Services.Interfaces;

namespace MerchStore.Application;

public static class DependencyInjection
{

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        
        services.AddScoped<ICatalogService, CatalogService>();

        return services;
    }
}