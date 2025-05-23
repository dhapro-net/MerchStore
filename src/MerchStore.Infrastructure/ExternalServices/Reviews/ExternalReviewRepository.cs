using MerchStore.Domain.Entities;
using MerchStore.Domain.Enums;
using MerchStore.Domain.Interfaces;
using MerchStore.Domain.ValueObjects;
using MerchStore.Infrastructure.ExternalServices.Reviews.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;

namespace MerchStore.Infrastructure.ExternalServices.Reviews;

/// <summary>
/// Repository implementation that integrates with the external review API
/// and implements circuit breaker pattern for resilience
/// </summary>
public class ExternalReviewRepository : IReviewRepository
{
    private readonly ReviewApiClient _apiClient;
    private readonly MockReviewService _mockReviewService;
    private readonly ILogger<ExternalReviewRepository> _logger;
    private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

    public ExternalReviewRepository(
        ReviewApiClient apiClient,
        MockReviewService mockReviewService,
        IOptions<ReviewApiOptions> options,
        ILogger<ExternalReviewRepository> logger)
    {
        _apiClient = apiClient;
        _mockReviewService = mockReviewService;
        _logger = logger;

        var circuitOptions = options.Value;

        // Configure the circuit breaker policy
        _circuitBreakerPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<TimeoutException>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: circuitOptions.ExceptionsAllowedBeforeBreaking,
                durationOfBreak: TimeSpan.FromSeconds(circuitOptions.CircuitBreakerDurationSeconds),
                onBreak: (ex, breakDuration) =>
                {
                    _logger.LogWarning(
                        "Circuit breaker opened for {BreakDuration} after {ExceptionType}: {ExceptionMessage}",
                        breakDuration, ex.GetType().Name, ex.Message);
                },
                onReset: () =>
                {
                    _logger.LogInformation("Circuit breaker reset, external API calls resumed");
                },
                onHalfOpen: () =>
                {
                    _logger.LogInformation("Circuit breaker half-open, testing external API");
                }
            );
    }

    public async Task<(IEnumerable<Review> Reviews, ReviewStats Stats)> GetProductReviewsAsync(Guid productId)
    {
        try
        {
            // Use circuit breaker to call the external API
            return await _circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                var response = await _apiClient.GetGroupReviewsAsync(productId);

                if (response?.Reviews == null || response.Stats == null)
                {
                    throw new InvalidOperationException("External API returned incomplete data");
                }

                // Map the external DTOs to domain entities
                var reviews = response.Reviews.Select(r => new Review(
                    Guid.Parse(r.Id ?? Guid.NewGuid().ToString()),
                    Guid.Parse(r.ProductId ?? productId.ToString()),
                    r.CustomerName ?? "Unknown",
                    r.Title ?? "No Title",
                    r.Content ?? "No Content",
                    r.Rating,
                    r.CreatedAt,
                    ParseReviewStatus(r.Status)
                )).ToList();

                var stats = new ReviewStats(
                    productId,
                    response.Stats.AverageRating,
                    response.Stats.ReviewCount
                );

                return (reviews, stats);
            });
        }
        catch (BrokenCircuitException)
        {
            _logger.LogWarning("Circuit is open, using mock review service for product {ProductId}", productId);
            return _mockReviewService.GetProductReviews(productId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reviews for product {ProductId}, falling back to mock data", productId);
            return _mockReviewService.GetProductReviews(productId);
        }
    }

    private static ReviewStatus ParseReviewStatus(string? status)
    {
        return status?.ToLowerInvariant() switch
        {
            "approved" => ReviewStatus.Approved,
            "rejected" => ReviewStatus.Rejected,
            "pending" => ReviewStatus.Pending,
            _ => ReviewStatus.Pending
        };
    }
}