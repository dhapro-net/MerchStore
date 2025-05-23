using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MerchStore.Infrastructure.ExternalServices.Reviews.Models;
using MerchStore.Infrastructure.ExternalServices.Reviews.Configurations;
using System.Text.Json;
using MerchStore.Domain.Interfaces;

namespace MerchStore.Infrastructure.ExternalServices.Reviews;

public class ReviewApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ReviewApiClient> _logger;
    private readonly ReviewApiOptions _options;

    private readonly IProductQueryRepository _productQueryRepository;


    // Options for pretty-printing JSON
    private static readonly JsonSerializerOptions _prettyJsonOptions = new() { WriteIndented = true };

    public ReviewApiClient(
        HttpClient httpClient,
        IOptions<ReviewApiOptions> options,
        ILogger<ReviewApiClient> logger,
         IProductQueryRepository productQueryRepository)
    {
        _httpClient = httpClient;
        _logger = logger;
        _options = options.Value;
        _productQueryRepository = productQueryRepository;

        // Configure the HttpClient
        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.DefaultRequestHeaders.Add(_options.ApiKeyHeaderName, _options.ApiKey);
        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
    }

    public async Task<ReviewResponseDto?> GetGroupReviewsAsync(Guid productId)
    {
        try
        {
            // 🧠 Step 1: Get product name from DB
            var expectedProductName = await _productQueryRepository.GetProductNameByIdAsync(productId);
            if (string.IsNullOrWhiteSpace(expectedProductName))
            {
                _logger.LogWarning("❌ Could not find product name for productId: {ProductId}", productId);
                return CreateEmptyResponse(productId);
            }

            // 🌐 Step 2: Call group review API
            string url = "api/v1/group-reviews?group=group5";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("API call failed: {StatusCode}", response.StatusCode);
                return CreateEmptyResponse(productId);
            }

            

            var productReviews = await response.Content.ReadFromJsonAsync<List<ProductReviewsDto>>();
            if (productReviews == null || !productReviews.Any())
            {
                _logger.LogInformation("API returned empty list");
                return CreateEmptyResponse(productId);
            }

            // 🔍 Step 3: Try match by productId
            var productReview = productReviews.FirstOrDefault(p =>
                !string.IsNullOrWhiteSpace(p.ProductId) &&
                Guid.TryParse(p.ProductId, out var id) &&
                id == productId);

            // 🔁 Step 4: Fallback to name match
            if (productReview == null)
            {
                productReview = productReviews.FirstOrDefault(p =>
                    !string.IsNullOrWhiteSpace(p.ProductName) &&
                    p.ProductName.Equals(expectedProductName, StringComparison.OrdinalIgnoreCase));
            }

            if (productReview == null || productReview.Reviews == null)
            {
                _logger.LogWarning("No matching product found for ID or name: {ProductId} / {ProductName}", productId, expectedProductName);
                return CreateEmptyResponse(productId);
            }

            // 🧮 Step 5: Extract data
            int reviewCount = productReview.Reviews?.Count ?? 0;
            var reviewDtos = productReview.Reviews?.Select(r => new ReviewDto
            {
                Id = Guid.NewGuid().ToString(),
                ProductId = productReview.ProductId,
                CustomerName = r.ReviewerName ?? "User",
                Title = r.ReviewTitle ?? "Review",
                Content = r.ReviewContent ?? "No content",
                Rating = r.Rating,
                CreatedAt = r.CreationDate,
                Status = "approved"
            }).ToList() ?? new List<ReviewDto>();

            double rating = reviewDtos.Any() ? reviewDtos.Average(r => r.Rating) : 0;

            return new ReviewResponseDto
            {
                Reviews = reviewDtos,
                Stats = new ReviewStatsDto
                {
                    ProductId = productReview.ProductId,
                    AverageRating = rating,
                    ReviewCount = reviewDtos.Count()
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching group reviews for productId {ProductId}", productId);
            return CreateEmptyResponse(productId);
        }
    }

    private ReviewResponseDto CreateEmptyResponse(Guid productId)
    {
        return new ReviewResponseDto
        {
            Reviews = new List<ReviewDto>(),
            Stats = new ReviewStatsDto
            {
                ProductId = productId.ToString(),
                AverageRating = 0,
                ReviewCount = 0
            }
        };
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