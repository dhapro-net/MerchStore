using MerchStore.Domain.Interfaces; // For IReviewRepository
using MerchStore.Domain.Entities; // For Review
using MerchStore.Domain.ValueObjects; // For ReviewStats
using Microsoft.Extensions.DependencyInjection; // For GetRequiredService
using Microsoft.Extensions.Logging;
using Xunit;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using MerchStore.Infrastructure.IntegrationTests; // For ILogger (optional)

namespace MerchStore.IntegrationTests;


// Contains integration tests for the ExternalReviewRepository,
// specifically testing its interaction with the live external review API.
// Uses the ReviewApiIntegrationTestFixture to get configured services.

public class ExternalReviewRepositoryIntegrationTests : IClassFixture<ReviewApiIntegrationTestFixture>
{
    // Service provider instance from the fixture
    private readonly IServiceProvider _serviceProvider;
    // The repository instance we will test, resolved via DI
    private readonly IReviewRepository _reviewRepository;
    // Optional logger for writing output during tests
    private readonly ILogger<ExternalReviewRepositoryIntegrationTests> _logger;

    // The fixture instance is injected by xUnit via the constructor
    public ExternalReviewRepositoryIntegrationTests(ReviewApiIntegrationTestFixture fixture)
    {
        _serviceProvider = fixture.ServiceProvider;
        // Resolve the IReviewRepository from the DI container set up by the fixture.
        // This gives us an instance of ExternalReviewRepository configured with ReviewApiClient etc.
        _reviewRepository = _serviceProvider.GetRequiredService<IReviewRepository>();
        // Resolve a logger instance (optional)
        _logger = _serviceProvider.GetRequiredService<ILogger<ExternalReviewRepositoryIntegrationTests>>();
    }

    [Fact] // Marks this as a test method
    [Trait("Category", "Integration")] // Mark as an Integration test
    [Trait("Category", "ExternalAPI")] // Mark specifically as hitting an external API
    public async Task GetProductReviewsAsync_WhenApiIsAvailable_ReturnsRealDataForKnownProduct()
    {
        // Arrange
        _logger.LogInformation("Starting test: GetProductReviewsAsync_WhenApiIsAvailable_ReturnsRealDataForKnownProduct");

        var knownProductId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"); // <<< --- CHANGE THIS GUID! ---
        _logger.LogInformation("Testing with known Product ID: {ProductId}", knownProductId);
        // --- --- --- ---

        // Act
        _logger.LogInformation("Calling _reviewRepository.GetProductReviewsAsync...");
        (IEnumerable<Review> Reviews, ReviewStats Stats) result;
        try
        {
             result = await _reviewRepository.GetProductReviewsAsync(knownProductId);
             _logger.LogInformation("Call completed. Received {ReviewCount} reviews.", result.Reviews?.Count() ?? 0);
        }
        catch (Exception ex)
        {
            // Log the exception if the call fails unexpectedly
            _logger.LogError(ex, "Call to GetProductReviewsAsync failed unexpectedly: {ErrorMessage}", ex.Message);
            // Rethrow or Assert.Fail to ensure the test fails clearly
            Assert.Fail($"GetProductReviewsAsync threw an unexpected exception: {ex.Message}");
            return; // Keep compiler happy, Assert.Fail prevents reaching here
        }

        // Assert
        _logger.LogInformation("Performing assertions...");
        // Basic checks to ensure data structure is as expected
        Assert.NotNull(result.Reviews); // Should be an empty list, not null, even if no reviews
        Assert.NotNull(result.Stats);   // Stats object should always be returned

        // Check if the ProductId in the stats matches the requested one
        Assert.Equal(knownProductId, result.Stats.ProductId);

        // More specific assertions (adapt based on expected data for your knownProductId):
        // - Is the review count non-negative?
        Assert.True(result.Stats.ReviewCount >= 0, $"Review count should be >= 0, but was {result.Stats.ReviewCount}");

        // - If you expect reviews for this specific product, check they are not empty
        //   (Comment out if the known product might legitimately have zero reviews)
        // Assert.NotEmpty(result.Reviews);

        // - Check if the average rating is within the valid range (0 to 5)
        Assert.InRange(result.Stats.AverageRating, 0.0, 5.0);

        // - If reviews exist, check some basic properties of the first review
        if (result.Reviews.Any())
        {
            var firstReview = result.Reviews.First();
            Assert.NotNull(firstReview.CustomerName);
            Assert.NotNull(firstReview.Title);
            Assert.NotNull(firstReview.Content);
            Assert.InRange(firstReview.Rating, 1, 5);
            Assert.Equal(knownProductId, firstReview.ProductId); // Ensure review belongs to the product
        }

        _logger.LogInformation("SUCCESS: Test passed for Product {ProductId}. Retrieved {ReviewCount} reviews. Avg Rating: {AverageRating}",
            knownProductId, result.Stats.ReviewCount, result.Stats.AverageRating);
    }
}