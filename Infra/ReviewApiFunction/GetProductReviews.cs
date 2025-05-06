using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReviewApiFunction.Models;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

namespace ReviewApiFunction
{
    public class GetProductReviews
    {
        private readonly ILogger<GetProductReviews> _logger;

        private static readonly Random _random = new Random();
        private static readonly string[] _customerNames = { "John Doe", "Jane Smith", "Bob Johnson", "Alice Brown", "Charlie Davis" };
        private static readonly string[] _reviewTitles = { "Great product!", "Highly recommended", "Exceeded expectations", "Not bad", "Could be better" };
        private static readonly string[] _reviewContents = {
            "I've been using this for weeks and it's fantastic.",
            "Exactly what I was looking for. High quality.",
            "The product is decent but shipping took too long.",
            "Works as advertised, very happy with my purchase.",
            "Good value for the money, would buy again."
        };

        public GetProductReviews(ILogger<GetProductReviews> logger)
        {
            _logger = logger;
        }

        [Function("GetProductReviews")]
        [OpenApiOperation(operationId: "GetProductReviews", tags: new[] { "Reviews" }, Summary = "Get product reviews", Description = "This retrieves all reviews for a specific product.")]
        [OpenApiParameter(name: "productId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "The ID of the product to get reviews for", Description = "The product ID must be a valid GUID")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ReviewResponse), Summary = "Successful operation", Description = "The product reviews were successfully retrieved")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid product ID", Description = "The product ID format was invalid")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Summary = "Internal server error", Description = "An unexpected error occurred processing the request")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products/{productId}/reviews")] HttpRequest req,
            string productId)
        {
            _logger.LogInformation("Processing request for product reviews.");

            try
            {
                // Validate product ID is a valid GUID
                if (!Guid.TryParse(productId, out Guid productGuid))
                {
                    return new BadRequestObjectResult(new { error = "Invalid product ID format. Must be a valid GUID." });
                }

                // Generate random number of reviews (0-5)
                int reviewCount = _random.Next(0, 6);
                List<Review> reviews = GenerateRandomReviews(productId, reviewCount);

                // Calculate average rating
                double averageRating = reviews.Any()
                    ? Math.Round(reviews.Average(r => r.Rating), 1)
                    : 0;

                // Create the response object
                var response = new ReviewResponse
                {
                    Reviews = reviews,
                    Stats = new ReviewStats
                    {
                        ProductId = productId,
                        AverageRating = averageRating,
                        ReviewCount = reviewCount
                    }
                };

                // Add a small delay to simulate network latency
                await Task.Delay(300);

                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing request: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        private static List<Review> GenerateRandomReviews(string productId, int count)
        {
            var reviews = new List<Review>();

            for (int i = 0; i < count; i++)
            {
                // Create a random date within the last 30 days
                var createdAt = DateTime.UtcNow.AddDays(-_random.Next(1, 31));

                // Generate a random rating, weighted toward positive reviews
                int rating = _random.Next(1, 101) switch
                {
                    var n when n <= 10 => 1, // 10% chance of 1-star
                    var n when n <= 25 => 2, // 15% chance of 2-stars
                    var n when n <= 50 => 3, // 25% chance of 3-stars
                    var n when n <= 80 => 4, // 30% chance of 4-stars
                    _ => 5                   // 20% chance of 5-stars
                };

                reviews.Add(new Review
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = productId,
                    CustomerName = _customerNames[_random.Next(_customerNames.Length)],
                    Title = _reviewTitles[_random.Next(_reviewTitles.Length)],
                    Content = _reviewContents[_random.Next(_reviewContents.Length)],
                    Rating = rating,
                    CreatedAt = createdAt,
                    Status = "approved"
                });
            }

            // Sort by date descending (newest first)
            return reviews.OrderByDescending(r => r.CreatedAt).ToList();
        }

        // 🔹 Endpoint: /api/products/{productId}/review-stats
        [Function("GetReviewStats")]
        public async Task<HttpResponseData> GetStats(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products/{productId}/review-stats")] HttpRequestData req,
            string productId,
            FunctionContext context)
        {
            var reviews = new[]
            {
            new { reviewer = "Alice", rating = 5 },
            new { reviewer = "Bob", rating = 4 },
            new { reviewer = "Carol", rating = 2 }
        };

            var avg = reviews.Average(r => r.rating);
            var count = reviews.Length;

            var stats = new
            {
                productId,
                averageRating = avg,
                reviewCount = count
            };

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(stats);
            return response;
        }
    }
}
