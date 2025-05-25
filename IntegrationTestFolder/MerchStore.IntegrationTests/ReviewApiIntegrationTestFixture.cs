using MerchStore.Domain.Interfaces;
using MerchStore.Infrastructure.ExternalServices.Reviews;
using MerchStore.Infrastructure.ExternalServices.Reviews.Configurations;
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
        // 1. Load configuration from appsettings.json
        Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // 2. Register services
        var services = new ServiceCollection();

        // Add logging (optional but useful for debugging tests)
        services.AddLogging(builder =>
        {
            builder.AddConfiguration(Configuration.GetSection("Logging"));
            builder.AddConsole();
            builder.AddDebug();
        });

        // 3. Register options from configuration
        services.Configure<ReviewApiOptions>(Configuration.GetSection(nameof(ReviewApiOptions)));

        // 4. Register HttpClientFactory and HttpClient (for ReviewApiClient)
        services.AddHttpClient();

        // 5. Register mocks or fakes
        services.AddSingleton<IProductQueryRepository, FakeProductQueryRepository>();
        services.AddSingleton<MockReviewService>(); // This is your internal fallback for reviews

        // 6. Register the actual services under test
        services.AddSingleton<ReviewApiClient>();
        services.AddSingleton<IReviewRepository, ExternalReviewRepository>();

        // 7. Build service provider
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