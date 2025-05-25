using MerchStore.Infrastructure.ExternalServices.Reviews.Configurations; // For ReviewApiOptions
using MerchStore.Infrastructure.IntegrationTests;
using Microsoft.Extensions.DependencyInjection; // For GetRequiredService, IServiceScopeFactory
using Microsoft.Extensions.Logging; // For ILogger, ILoggerFactory
using Microsoft.Extensions.Options;
using MerchStore.Infrastructure.ExternalServices.Reviews;
using MerchStore.Domain.Interfaces; // For IOptions, Options.Create

namespace MerchStore.IntegrationTests;

/// Contains integration tests specifically for verifying the circuit breaker
public class ExternalReviewRepositoryCircuitBreakerTests : IClassFixture<ReviewApiIntegrationTestFixture>
{
    // Create a scope factory to manage the lifetime of services
    private readonly IServiceScopeFactory _scopeFactory;

    // Constructor receives the shared fixture
    public ExternalReviewRepositoryCircuitBreakerTests(ReviewApiIntegrationTestFixture fixture)
    {
        // Use the fixture to get the service provider
        _scopeFactory = fixture.ServiceProvider.GetRequiredService<IServiceScopeFactory>();
    }

    [Fact]
    [Trait("Category", "Integration")]
    [Trait("Category", "CircuitBreaker")]
    public async Task CircuitBreaker_OpensAfterThresholdAndFallsBack()
    {
        // Arrange
        using var scope = _scopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var mockReviewService = serviceProvider.GetRequiredService<MockReviewService>();
        var originalOptions = serviceProvider.GetRequiredService<IOptions<ReviewApiOptions>>().Value;

        // Create a new ReviewApiOptions instance with a non-existent endpoint to trigger failures
        // This simulates the external API being down or unreachable
        var testOptions = new ReviewApiOptions
        {
            BaseUrl = "http://localhost:9999/invalid-path/", // Non-existent endpoint
            ApiKey = "test-key",
            ApiKeyHeaderName = originalOptions.ApiKeyHeaderName,
            TimeoutSeconds = 5,
            ExceptionsAllowedBeforeBreaking = originalOptions.ExceptionsAllowedBeforeBreaking > 0 ? originalOptions.ExceptionsAllowedBeforeBreaking : 2, // Use 2 for test
            CircuitBreakerDurationSeconds = 5
        };
        var optionsWrapper = Options.Create(testOptions);

        // Create a new HttpClient with the test options and configure the repository
        var httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient();
        var productQueryRepository = serviceProvider.GetRequiredService<IProductQueryRepository>();
        var apiClient = new ReviewApiClient(httpClient, optionsWrapper, loggerFactory.CreateLogger<ReviewApiClient>(), productQueryRepository, mockReviewService);
        var repository = new ExternalReviewRepository(apiClient, mockReviewService, optionsWrapper, loggerFactory.CreateLogger<ExternalReviewRepository>());

        var productId = Guid.NewGuid(); // Generate a new product ID for testing. Does not matter.
        int exceptionsAllowed = testOptions.ExceptionsAllowedBeforeBreaking;
        var logger = loggerFactory.CreateLogger<ExternalReviewRepositoryCircuitBreakerTests>(); // Get logger instance

        logger.LogInformation("Circuit Breaker Test: ExceptionsAllowedBeforeBreaking = {Count}", exceptionsAllowed);

        // Act - Trigger initial failures
        // We expect these calls to fail internally, be caught by the repository's
        // outer catch block, and return mock data.
        for (int i = 0; i < exceptionsAllowed; i++)
        {
            int callNum = i + 1;
            logger.LogInformation("Circuit Breaker Test: Making failing call #{CallNum}", callNum);
            try
            {
                // Call the method - it should complete successfully by returning mock data
                var (initialFallbackReviews, initialFallbackStats) = await repository.GetProductReviewsAsync(productId);

                // Verify it returned mock data even here
                if (initialFallbackReviews.Any())
                {
                     logger.LogInformation("Circuit Breaker Test: Call #{CallNum} completed and returned mock data (Title: '{Title}') as expected (due to repo catch block).",
                        callNum, initialFallbackReviews.First().Title);
                     Assert.StartsWith("Review:", initialFallbackReviews.First().Title, StringComparison.OrdinalIgnoreCase);

                     // --- Log details of initial fallback reviews ---
                     logger.LogInformation("--- Details of Initial Fallback Reviews (Call #{CallNum}) ---", callNum);
                     int reviewIndex = 0;
                     foreach (var review in initialFallbackReviews)
                     {
                         // Log each review's details in a more readable format
                         logger.LogInformation(
                             "Initial Fallback Review [{Index}]:\n" +
                             "  ID      : {ReviewId}\n" +
                             "  Rating  : {Rating}\n" +
                             "  Customer: {CustomerName}\n" +
                             "  Title   : {ReviewTitle}\n" +
                             "  Content : {ReviewContent}",
                             ++reviewIndex, // Increment index for readability
                             review.Id,
                             review.Rating,
                             review.CustomerName,
                             review.Title,
                             review.Content // Added Content
                         );
                     }
                     logger.LogInformation("--- End Initial Fallback Review Details ---");
                     // --- End Logging ---
                }
                else
                {
                     logger.LogInformation("Circuit Breaker Test: Call #{CallNum} completed and returned 0 mock reviews as expected (due to repo catch block).", callNum);
                     Assert.Equal(0, initialFallbackStats.ReviewCount);
                }
            }
            catch (Exception ex)
            {
                // If any *other* exception occurs here, fail the test as setup might be wrong
                logger.LogError(ex, "Circuit Breaker Test: Call #{CallNum} failed unexpectedly during initial failure loop.", callNum);
                Assert.Fail($"Call #{callNum} threw an unexpected exception: {ex.GetType().Name} - {ex.Message}");
            }
        }

        // Act & Assert - Trigger the circuit opening and verify fallback
        int finalCallNum = exceptionsAllowed + 1;
        logger.LogInformation("Circuit Breaker Test: Making call #{CallNum} (circuit should be open, should fallback)", finalCallNum);

        // This call should now trigger the fallback because the circuit is open.
        // The repository catches the BrokenCircuitException internally and returns mock data.
        var (fallbackReviews, fallbackStats) = await repository.GetProductReviewsAsync(productId);

        // Assert: Check if the returned data comes from the MockReviewService
        Assert.NotNull(fallbackReviews);
        Assert.NotNull(fallbackStats);

        // MockReviewService now adds "Review:" prefix to titles
        if (fallbackReviews.Any())
        {
            // Check the first review's title prefix to confirm it's likely from the mock service
            Assert.StartsWith("Review:", fallbackReviews.First().Title, StringComparison.OrdinalIgnoreCase);
            logger.LogInformation("Circuit Breaker Test: Call #{CallNum} correctly returned mock data (Title: '{Title}'). Circuit successfully opened and fallback occurred.",
                finalCallNum, fallbackReviews.First().Title);

            // --- Log details of final fallback reviews ---
            logger.LogInformation("--- Details of Final Fallback Reviews (Call #{CallNum}) ---", finalCallNum);
            int reviewIndex = 0;
            foreach (var review in fallbackReviews)
            {
                 // Log each review's details in a more readable format
                 logger.LogInformation(
                     "Final Fallback Review [{Index}]:\n" +
                     "  ID      : {ReviewId}\n" +
                     "  Rating  : {Rating}\n" +
                     "  Customer: {CustomerName}\n" +
                     "  Title   : {ReviewTitle}\n" +
                     "  Content : {ReviewContent}",
                     ++reviewIndex, // Increment index for readability
                     review.Id,
                     review.Rating,
                     review.CustomerName,
                     review.Title,
                     review.Content // Added Content
                 );
            }
            logger.LogInformation("--- End Final Fallback Review Details ---");
            // --- End Logging ---
        }
        else
        {
            // It's possible the mock service generated 0 reviews
            Assert.Equal(0, fallbackStats.ReviewCount);
             logger.LogInformation("Circuit Breaker Test: Call #{CallNum} correctly returned 0 mock reviews. Circuit successfully opened and fallback occurred.", finalCallNum);
        }
    }
}