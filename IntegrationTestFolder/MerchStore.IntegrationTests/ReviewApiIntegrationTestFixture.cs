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
        // Step 1: Load appsettings.json for configuration
        Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // Step 2: Set up the service collection (DI container)
        var services = new ServiceCollection();

        // Step 3: Register logging services (for debug/log output)
        services.AddLogging(builder =>
        {
            builder.AddConfiguration(Configuration.GetSection("Logging"));
            builder.AddConsole();
            builder.AddDebug();
        });

        // Step 4: ✅ Register the fake product repository (used in integration tests)
        services.AddSingleton<IProductQueryRepository, FakeProductQueryRepository>();

        // Step 5: Register review services (includes ReviewApiClient, ExternalReviewRepository, etc.)
        services.AddReviewServices(Configuration);

        // Step 6: Build the service provider
        ServiceProvider = services.BuildServiceProvider();
    }

    // Step 7: Dispose pattern for cleaning up resources
    public void Dispose()
    {
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}