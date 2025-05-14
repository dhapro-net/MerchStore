namespace MerchStore.Domain.ValueObjects;

public record ReviewStats
{
    public Guid ProductId { get; }
    public double AverageRating { get; }
    public int ReviewCount { get; }

    public ReviewStats(Guid productId, double averageRating, int reviewCount)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("Product ID cannot be empty", nameof(productId));

        if (averageRating < 0 || averageRating > 5)
            throw new ArgumentOutOfRangeException(nameof(averageRating), "Average rating must be between 0 and 5");

        if (reviewCount < 0)
            throw new ArgumentOutOfRangeException(nameof(reviewCount), "Review count cannot be negative");

        ProductId = productId;
        AverageRating = averageRating;
        ReviewCount = reviewCount;
    }
}

//Logic : set validation rule to express domain constraints.