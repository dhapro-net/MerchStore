using System.Reflection;
using MerchStore.Domain.Entities;
using MerchStore.Domain.Enums;
using MerchStore.Domain.Interfaces;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.IntegrationTests;

public class FakeProductQueryRepository : IProductQueryRepository
{
    public Task<IEnumerable<Product>> GetAllProductsAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IEnumerable<Product>>(new List<Product>());
    }

    public Task<Product?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken)
    {
        var product = new Product(productId, "Fake Product", "Test Desc", "Test", "image.jpg", new Money(100m, "SEK"), 10);
        return Task.FromResult<Product?>(product);
    }

    public Task<IEnumerable<Product>> GetFeaturedProductsAsync()
    {
        return Task.FromResult<IEnumerable<Product>>(new List<Product>());
    }

    public Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
    {
        return Task.FromResult<IEnumerable<Product>>(new List<Product>());
    }

    public Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
    {
        return Task.FromResult<IEnumerable<Product>>(new List<Product>());
    }

    public Task<IEnumerable<Product>> GetProductsInPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        return Task.FromResult<IEnumerable<Product>>(new List<Product>());
    }

    public Task<string?> GetGroupReviewsAsync(Guid productId)
    {
        return Task.FromResult<string?>("Mocked group review data");
    }

    public Task<string?> GetProductNameByIdAsync(Guid productId)
    {
        return Task.FromResult<string?>("Mocked Product Name");
    }

    // ✅ THIS FIXES YOUR TEST
    public Task<IEnumerable<Review>> GetReviewsByProductIdAsync(Guid productId)
    {
        var now = DateTime.UtcNow;

        var reviews = new List<Review>
        {
            CreateReview(productId, "Alice", "Sample Review: Great!", "Loved it", 5, now),
            CreateReview(productId, "Bob", "Sample Review: Good", "Pretty solid", 4)
        };

        return Task.FromResult<IEnumerable<Review>>(reviews);
    }

    private Review CreateReview(Guid productId, string customerName, string title, string content, int rating, DateTime? createdAt = null)
    {
        var review = (Review)Activator.CreateInstance(
            typeof(Review),
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            args: null,
            culture: null
        )!;

        var id = Guid.NewGuid();
        var created = createdAt ?? DateTime.UtcNow;

        SetPrivateProperty(review, "Id", id);
        SetPrivateProperty(review, nameof(Review.ProductId), productId);
        SetPrivateProperty(review, nameof(Review.CustomerName), customerName);
        SetPrivateProperty(review, nameof(Review.Title), title);
        SetPrivateProperty(review, nameof(Review.Content), content);
        SetPrivateProperty(review, nameof(Review.Rating), rating);
        SetPrivateProperty(review, nameof(Review.CreatedAt), created);
        SetPrivateProperty(review, nameof(Review.Status), ReviewStatus.Approved);

        return review;
    }

    private void SetPrivateProperty<T>(Review obj, string propertyName, T value)
    {
        typeof(Review)
            .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            ?.SetValue(obj, value);
    }
}