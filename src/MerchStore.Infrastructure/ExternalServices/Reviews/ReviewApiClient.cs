using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MerchStore.Infrastructure.ExternalServices.Reviews.Models;
using MerchStore.Infrastructure.ExternalServices.Reviews.Configurations;
using System.Text.Json;
using MerchStore.Domain.Interfaces;
using MerchStore.Infrastructure.ExternalServices.Reviews.Helpers;
using MerchStore.Application.DTOs;

namespace MerchStore.Infrastructure.ExternalServices.Reviews;

public class ReviewApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ReviewApiClient> _logger;
    private readonly ReviewApiOptions _options;
    private readonly IProductQueryRepository _productQueryRepository;
    private readonly MockReviewService _mockReviewService;


    // Options for pretty-printing JSON
    private static readonly JsonSerializerOptions _prettyJsonOptions = new() { WriteIndented = true };

    public ReviewApiClient(
        HttpClient httpClient,
        IOptions<ReviewApiOptions> options,
        ILogger<ReviewApiClient> logger,
         IProductQueryRepository productQueryRepository,
         MockReviewService mockReviewService)
    {
        _httpClient = httpClient;
        _logger = logger;
        _options = options.Value;
        _productQueryRepository = productQueryRepository;
        _mockReviewService = mockReviewService;

        // Configure the HttpClient
        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.DefaultRequestHeaders.Add(_options.ApiKeyHeaderName, _options.ApiKey);
        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
    }

    public async Task<ReviewResponseDto?> GetGroupReviewsAsync(Guid productId)
    {
        try
        {
            //  Step 1: Get product name from DB
            var expectedProductName = await _productQueryRepository.GetProductNameByIdAsync(productId);
            if (string.IsNullOrWhiteSpace(expectedProductName))
            {
                _logger.LogWarning("❌ Could not find product name for productId: {ProductId}", productId);
                var (mockReviews, _) = _mockReviewService.GetProductReviews(productId);
                var fallbackResult = UseMockFallback.ToDto(productId, mockReviews);
                fallbackResult.Source = "mock";
                return fallbackResult;
            }

            // Step 2: Get reviews from external API 
            var url = "api/v1/group-reviews?group=group5";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("❌ API call failed: {StatusCode}", response.StatusCode);
                var (mockReviews, _) = _mockReviewService.GetProductReviews(productId);
                var fallbackResult = UseMockFallback.ToDto(productId, mockReviews);
                fallbackResult.Source = "mock";
                return fallbackResult;
            }

            var productReviews = await response.Content.ReadFromJsonAsync<List<ProductReviewsDto>>();
            if (productReviews == null || !productReviews.Any())
            {
                _logger.LogInformation("⚠️ API returned empty list");
                var (mockReviews, _) = _mockReviewService.GetProductReviews(productId);
                var fallbackResult = UseMockFallback.ToDto(productId, mockReviews);
                fallbackResult.Source = "mock";
                return fallbackResult;
            }

            //  Step 3: Try match by productId
            var productReview = productReviews.FirstOrDefault(p =>
                !string.IsNullOrWhiteSpace(p.ProductId) &&
                Guid.TryParse(p.ProductId, out var id) &&
                id == productId);

            // Step 4: Fallback to name match
            if (productReview == null)
            {
                productReview = productReviews.FirstOrDefault(p =>
                    !string.IsNullOrWhiteSpace(p.ProductName) &&
                    p.ProductName.Equals(expectedProductName, StringComparison.OrdinalIgnoreCase));
            }

            //  Step 5: No match or empty → use fallback
            if (productReview?.Reviews == null || !productReview.Reviews.Any())
            {
                _logger.LogWarning("⚠️ No reviews found for product {ProductId}, using fallback", productId);
                return _mockReviewService.EnsureMockReviewsIfMissing(productId);
            }

            // Step 6: Extract data
            
            var reviewDtos = productReview.Reviews.Select(r => new ReviewProductDto
            {
                Id = Guid.NewGuid().ToString(),
                ProductId = productReview.ProductId,
                CustomerName = r.ReviewerName ?? "User",
                Title = r.ReviewTitle ?? "Review",
                Content = r.ReviewContent ?? "No content",
                Rating = r.Rating,
                CreatedAt = r.CreationDate,
                Status = "approved"
            }).ToList();

            double rating = reviewDtos.Any() ? reviewDtos.Average(r => r.Rating) : 0;

            return new ReviewResponseDto
            {
                Reviews = reviewDtos,
                Stats = new ReviewStatsDto
                {
                    ProductId = productReview.ProductId,
                    AverageRating = rating,
                    ReviewCount = reviewDtos.Count
                },
                Source = "real"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔥 External API error. Using fallback for productId {ProductId}", productId);
            var (mockReviews, _) = _mockReviewService.GetProductReviews(productId);
            var fallbackResult = UseMockFallback.ToDto(productId, mockReviews);
            fallbackResult.Source = "mock";
            return fallbackResult;
        }
    }

    //helper method to parse the rating
    public double ParseFormattedRating(string? formattedRating)
    {
        if (string.IsNullOrWhiteSpace(formattedRating))
            return 0;

        int start = formattedRating.IndexOf('(');
        int end = formattedRating.IndexOf(' ', start);

        if (start >= 0 && end > start)
        {
            string number = formattedRating.Substring(start + 1, end - start - 1);
            double.TryParse(number, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double parsed);
            return parsed;
        }

        return 0;
    }
}