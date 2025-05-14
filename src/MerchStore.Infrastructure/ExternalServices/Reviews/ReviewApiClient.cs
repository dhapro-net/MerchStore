using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MerchStore.Infrastructure.ExternalServices.Reviews.Models;
using MerchStore.Infrastructure.ExternalServices.Reviews.Configurations;
using System.Text.Json;

namespace MerchStore.Infrastructure.ExternalServices.Reviews;

public class ReviewApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ReviewApiClient> _logger;
    private readonly ReviewApiOptions _options;

    // Options for pretty-printing JSON
    private static readonly JsonSerializerOptions _prettyJsonOptions = new() { WriteIndented = true };

    public ReviewApiClient(
        HttpClient httpClient,
        IOptions<ReviewApiOptions> options,
        ILogger<ReviewApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _options = options.Value;

        // Configure the HttpClient
        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.DefaultRequestHeaders.Add(_options.ApiKeyHeaderName, _options.ApiKey);
        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
    }

    public async Task<ReviewResponseDto?> GetProductReviewsAsync(Guid productId)
    {
        try
        {
            string url = $"products/{productId}/reviews";

            _logger.LogInformation("Requesting reviews for product {ProductId} from external API", productId);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var reviewsResponse = await response.Content.ReadFromJsonAsync<ReviewResponseDto>();

            _logger.LogInformation("Successfully retrieved {ReviewCount} reviews for product {ProductId}",
                reviewsResponse?.Reviews?.Count ?? 0, productId);

            // Log pretty-printed JSON response in debug mode
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                var json = JsonSerializer.Serialize(reviewsResponse, _prettyJsonOptions);
                _logger.LogDebug("Received response: {Json}", json);
            }

            return reviewsResponse;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while fetching reviews for product {ProductId}: {Message}",
                productId, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching reviews for product {ProductId}: {Message}",
                productId, ex.Message);
            throw;
        }
    }
}