using MerchStore.Domain.Entities;
using MerchStore.Domain.Enums;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.Infrastructure.ExternalServices.Reviews;

/// <summary>
/// Provides mock review data for use as a fallback when the external API is unavailable
/// </summary>
public class MockReviewService
{
    private static readonly Random _random = new Random();
    private static readonly string[] _customerNames = { "John Doe", "Jane Smith", "Bob Johnson", "Alice Brown", "Charlie Davis" };
    private static readonly string[] _reviewContents = {
        "I've been using this for weeks and it's fantastic.",
        "Exactly what I was looking for. High quality.",
        "The product is decent but shipping took too long.",
        "Works as advertised, very happy with my purchase.",
        "Good value for the money, would buy again."
    };

    /// <summary>
    /// Generates mock reviews for a product
    /// </summary>
    public (IEnumerable<Review> Reviews, ReviewStats Stats) GetProductReviews(Guid productId)
    {
        // Generate a consistent number of reviews based on product ID
        // This ensures the same product always gets the same number of reviews
        var productIdHash = productId.GetHashCode();
        var reviewCount = Math.Abs(productIdHash % 6); // 0-5 reviews

        var reviews = GenerateMockReviews(productId, reviewCount);

        // Calculate average rating
        double averageRating = reviews.Any()
            ? Math.Round(reviews.Average(r => r.Rating), 1)
            : 0;

        var stats = new ReviewStats(productId, averageRating, reviewCount);

        return (reviews, stats);
    }

    private IEnumerable<Review> GenerateMockReviews(Guid productId, int count)
    {
        var reviews = new List<Review>();
        var productSeed = productId.GetHashCode();
        var random = new Random(productSeed); // Use product ID as seed for consistency

        for (int i = 0; i < count; i++)
        {
            // Use a deterministic approach to generate review data
            int dayOffset = random.Next(1, 31);
            var createdAt = DateTime.UtcNow.AddDays(-dayOffset);

            int ratingBase = random.Next(1, 101);
            int rating = ratingBase switch
            {
                var n when n <= 10 => 1, // 10% chance of 1-star
                var n when n <= 25 => 2, // 15% chance of 2-stars
                var n when n <= 50 => 3, // 25% chance of 3-stars
                var n when n <= 80 => 4, // 30% chance of 4-stars
                _ => 5                   // 20% chance of 5-stars
            };

            string title = $"Sample Review: {i+1} for Product";
            string customerName = _customerNames[random.Next(_customerNames.Length)];
            string content = _reviewContents[random.Next(_reviewContents.Length)];

            reviews.Add(new Review(
                Guid.NewGuid(),
                productId,
                customerName,
                title,
                content,
                rating,
                createdAt,
                ReviewStatus.Approved
            ));
        }

        // Sort by date descending (newest first)
        return reviews.OrderByDescending(r => r.CreatedAt).ToList();
    }
}

//This mock service will provide synthetic review data that can be used when the real API fails.