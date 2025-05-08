using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using ReviewApiFunction;
using Microsoft.Extensions.DependencyInjection;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {

        services.AddApplicationInsightsTelemetryWorkerService();
        //services.ConfigureApplicationInsights(); // Removed as the method is not defined
        // Add OpenAPI configuration using our custom classes
        services.AddSingleton<IOpenApiConfigurationOptions, SwaggerConfiguration>();

        // Add custom UI options
        services.AddSingleton<IOpenApiCustomUIOptions, SwaggerUIConfiguration>();


    })
    .Build();
host.Run();


