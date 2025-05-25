using MerchStore.Domain.Interfaces;
using MerchStore.IntegrationTests;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MerchStore.Infrastructure.IntegrationTests;


// Sets up the necessary configuration and dependency injection container
// for running integration tests against the real External Review API.
///This fixture is created once per test class.

public class ReviewApiIntegrationTestFixture : IDisposable
{
    public IServiceProvider ServiceProvider { get; private set; }
    public IConfiguration Configuration { get; private set; }

    public ReviewApiIntegrationTestFixture()
    {
        // Build configuration
        Configuration = new ConfigurationBuilder()
            // Set base path to the directory where the test assembly is running
            .SetBasePath(Directory.GetCurrentDirectory())
            // Add the appsettings.json file we created for tests
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            // Optionally add environment variables or other sources
            .Build();

        // Set up dependency injection
        var services = new ServiceCollection();
        services.AddScoped<IProductQueryRepository, FakeProductQueryRepository>();

        // Add logging (optional, but helpful for debugging)
        // This configures logging providers based on the "Logging" section in appsettings.json
        services.AddLogging(builder =>
        {
            builder.AddConfiguration(Configuration.GetSection("Logging"));
            // Add common providers - output will show in test runner console/debug output
            builder.AddConsole();
            builder.AddDebug();
        });

      
        services.AddReviewServices(Configuration);
        // --- --- --- --- --- --- --- --- ---

        // Build the service provider
        ServiceProvider = services.BuildServiceProvider();
    }

       public void Dispose()
    {

        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
        GC.SuppressFinalize(this);
    }
}